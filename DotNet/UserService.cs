using ReCycle.Data;
using ReCycle.Data.Providers;
using ReCycle.Models;
using ReCycle.Models.Domain;
using ReCycle.Models.Enums;
using ReCycle.Models.Requests;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ReCycle.Services
{
    public class UserService : IUserService
    {
        private IAuthenticationService<int> _authenticationService;
        private IDataProvider _dataProvider;

        public UserService(IAuthenticationService<int> authService, IDataProvider dataProvider)
        {
            _authenticationService = authService;
            _dataProvider = dataProvider;
        }
		
		#region User
        public async Task<bool> LogInAsync(UserLogRequest logModel)
        {
            bool isSuccessful = false;

            UserAuthData response = GetAuth(logModel);

            if (response != null)
            {
                isSuccessful = await LogIn(response);
            }

            return isSuccessful;
        }

        public async Task<bool> LogIn(UserAuthData model)
        {
            bool isSuccessful = false;

            if (model != null)
            {
                string name = model.Email;

                IUserAuthData response = new UserBase
                {
                    Id = model.Id,
                    Name = name,
                    Roles = model.Roles.Select(item => item.Name),
                    TenantId = "ReCycle User"
                };

                isSuccessful = true;

                Claim fullName = new Claim("CustomClaim", "ReCycle User");
                await _authenticationService.LogInAsync(response, new Claim[] { fullName });
            }
            return isSuccessful;
        }

        public UserAuthData GetAuth(UserLogRequest logModel)
        {
            string passwordFromDb = "";
            UserAuthData authModel = null;
            Dictionary<int, List<Role>> userRoles = null;

            _dataProvider.ExecuteCmd("dbo.Users_Select_AuthData", inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Email", logModel.Email);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int index;

                    switch (set)
                    {
                        case 0:

                            index = 0;

                            int roleId = reader.GetSafeInt32(index++);
                            string roleName = reader.GetSafeString(index++);
                            int userId = reader.GetSafeInt32(index++);

                            if (userRoles == null)
                            {
                                userRoles = new Dictionary<int, List<Role>>();
                            }

                            Role role = new Role() { Id = roleId, Name = roleName };
                            if (userRoles.ContainsKey(userId))
                            {
                                userRoles[userId].Add(role);
                            }
                            else
                            {
                                userRoles.Add(userId, new List<Role> { role });
                            }
                            break;

                        case 1:

                            passwordFromDb = reader.GetSafeString(2);
                            bool isValidCredentials = BCrypt.BCryptHelper.CheckPassword(logModel.Password, passwordFromDb);

                            if (isValidCredentials)
                            {
                                authModel = new UserAuthData();
                                index = 0;

                                authModel.Id = reader.GetSafeInt32(index++);
                                authModel.Email = reader.GetSafeString(index++);
                                authModel.Password = reader.GetSafeString(index++);
                                authModel.UserStatusId = reader.GetSafeInt32(index++);

                                if (userRoles.ContainsKey(authModel.Id))
                                {
                                    authModel.Roles = userRoles[authModel.Id];
                                }
                            }
                            break;
                    }
                }
             );
            return authModel;
        }

        public void UpdateUserStatusId(int userId, int userStatusId)
        {
            _dataProvider.ExecuteNonQuery("dbo.Users_UpdateStatus", inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@UserStatusId", userStatusId);
                col.AddWithValue("@Id", userId);
            },
            returnParameters: null);
        }

        public void UpdateUserPassword(int userId, string password)
        {
            string hashedPassword = HashPassword(password);
            _dataProvider.ExecuteNonQuery("dbo.Users_UpdatePassword", inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Password", hashedPassword);
                col.AddWithValue("@Id", userId);
            },
            returnParameters: null);
        }

        public bool VerifyToken(string token)
        {
            string procName = "[dbo].[Select_ByToken]";
            bool isVerified = false;

            int userId = 0;

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Token", token);
            }, delegate (IDataReader reader, short set)
            {
                userId = reader.GetSafeInt32(1);
            }
            );
            if (userId > 0)
            {
                isVerified = true;
                UpdateUserStatusId(userId, (int)UserStatus.Active);
            }

            return isVerified;
        }

        public int ResetPassword(string token)
        {
            string procName = "[dbo].[Select_ByToken]";

            int userId = 0;

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Token", token);
            }, delegate (IDataReader reader, short set)
            {
                userId = reader.GetSafeInt32(1);
            }
            );
            return userId;
        }

        public void DeleteToken(string token)
        {
            string procName = "[dbo].[UserTokens_Delete]";

            _dataProvider.ExecuteNonQuery(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Token", token);
            });
        }

        public User GetByEmail(string email)
        {
            User user = new User();

            _dataProvider.ExecuteCmd("dbo.Users_SelectPass_ByEmail", delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Email", email);

            }, delegate (IDataReader reader, short set)
            {
                int index = 0;
                user.Id = reader.GetSafeInt32(index++);
                user.Email = reader.GetSafeString(index++);
                user.Password = reader.GetSafeString(index++);
                user.UserStatusId = reader.GetSafeInt32(index++);

            }
            );
            return user;
        }

        public int Create(UserAddRequest model)
        {
            string password = model.Password;
            string role = model.UserRole;

            string salt = BCrypt.BCryptHelper.GenerateSalt();
            string hashedPassword = BCrypt.BCryptHelper.HashPassword(password, salt);

            int id = 0;

            _dataProvider.ExecuteNonQuery("dbo.Users_Insert", inputParamMapper: delegate (SqlParameterCollection col)
              {
                  col.AddWithValue("@Email", model.Email);
                  col.AddWithValue("@Password", hashedPassword);

                  if (role == "Seller")
                  {
                      col.AddWithValue("@UserRole", (int)UserRole.Seller);
                  }
                  else
                  {
                      col.AddWithValue("@UserRole", (int)UserRole.User);
                  }
                  SqlParameter idOut = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                  idOut.Direction = ParameterDirection.Output;

                  col.Add(idOut);
              },
              returnParameters: delegate (SqlParameterCollection returnCollection)
              {
                  object oId = returnCollection["@Id"].Value;

                  int.TryParse(oId.ToString(), out id);

              });
            return id;
        }
		#endregion
		
		#region Admin
		public void Update(UserUpdateRequest model)
        {
            _dataProvider.ExecuteNonQuery("dbo.Users_Update", inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Email", model.Email);
                col.AddWithValue("@IsConfirmed", model.IsConfirmed);
                col.AddWithValue("@UserStatusId", model.UserStatusId);
                col.AddWithValue("@Id", model.Id);
            },
            returnParameters: null);
        }
		
        public Paged<User> Get(int pageIndex, int pageSize)
        {
            Paged<User> pagedResult = null;

            List<User> result = null;

            int totalCount = 0;

            _dataProvider.ExecuteCmd("dbo.Users_SelectAll", inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@pageIndex", pageIndex);
                paramCollection.AddWithValue("@pageSize", pageSize);
            },
            singleRecordMapper: delegate (IDataReader reader, short set)
            {
                User aUser = MapUser(reader);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(3);
                }
                if (result == null)
                {
                    result = new List<User>();
                }
                result.Add(aUser);
            }
            );
            if (result != null)
            {
                pagedResult = new Paged<User>(result, pageIndex, pageSize, totalCount);
            }
            return pagedResult;
        }
		
        public User Get(int id)
        {
            User user = null;

            _dataProvider.ExecuteCmd("dbo.Users_SelectById", delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                user = MapUser(reader);
            }
            );
            return user;
        }
		#endregion

        private static User MapUser(IDataReader reader)
        {
            User aUser = new User();

            int index = 0;

            aUser.Id = reader.GetSafeInt32(index++);
            aUser.Email = reader.GetSafeString(index++);
            aUser.UserStatusId = reader.GetSafeInt32(index++);

            return aUser;
        }

        private string HashPassword(string password)
        {
            string salt = BCrypt.BCryptHelper.GenerateSalt();
            string hashedPassword = BCrypt.BCryptHelper.HashPassword(password, salt);
            return hashedPassword;
        }
    }

}