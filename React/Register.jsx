import React from "react";
import * as userService from "../../services/userService";
import logger from "recycle-debug";
import { Formik, FastField, Field, Form } from "formik";
import * as userSchema from "./userValidationSchema";
import PropTypes from "prop-types";
import Swal from "sweetalert";

const _logger = logger.extend("Register");

class Register extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      formData: {
        email: "",
        password: "",
        passwordConfirm: "",
        userRole: "",
        terms: false
      }
    };
  }

  handleSubmit = (formValues, { resetForm }) => {
    this.setState(
      () => {
        return {
          formData: formValues
        };
      },
      () =>
        userService
          .create(this.state.formData)
          .then(this.onSubmitSuccess)
          .then(() => {
            resetForm({
              email: "",
              password: "",
              passwordConfirm: "",
              userRole: "",
              terms: false
            });
          })
          .catch(this.onSubmitError)
    );
  };

  onSubmitSuccess = success => {
    Swal(
      "Registration Complete!",
      "An activation email has been sent to you.",
      "success"
    );
    _logger(success);
  };

  onSubmitError = error => {
    Swal(
      "Something went wrong!",
      "Please check the form as an account with this information may already exist",
      "error"
    );
    _logger(error);
  };

  toLogin = () => {
    this.props.history.push("/login");
  };

  render() {
    const footer = {
      backgroundColor: "#e8e8e8"
    };
    return (
      <>
        <Formik
          initialValues={this.state.formData}
          onSubmit={this.handleSubmit}
          validationSchema={userSchema.regSchema}
          eneableReinitialize={true}
          render={({ errors, touched }) => (
            <Form>
              <div className="block-center mt-4 wd-xl">
                <div className="card card-flat">
                  <div className="card-header text-center bg-dark">
                    <span>
                      <img
                        className="block-center rounded"
                        src="./img/home/re-cycle-logo.png"
                        alt="Logo"
                        style={{ width: "50%", height: "auto" }}
                      />
                    </span>
                  </div>
                  <div className="card-body">
                    <p className="text-center py-2">
                      SIGNUP TO GET INSTANT ACCESS.
                    </p>
                    <div className="mb-3" name="formRegister">
                      <div className="form-group">
                        <label
                          className="text-muted"
                          htmlFor="signupInputEmail1"
                        >
                          Email address
                        </label>
                        <div className="input-group with-focus">
                          <FastField
                            name="email"
                            type="email"
                            className="border-right-0 form-control"
                            component="input"
                            placeholder="Enter an email"
                          />
                          <div className="input-group-append">
                            <span className="input-group-text text-muted bg-transparent border-left-0">
                              <em className="fa fa-envelope" />
                            </span>
                          </div>
                        </div>
                        {touched.email && errors.email && (
                          <div className="text-danger">{errors.email}</div>
                        )}
                      </div>
                      <div className="form-group">
                        <label
                          className="text-muted"
                          htmlFor="signupInputPassword1"
                        >
                          Password
                        </label>
                        <div className="input-group with-focus">
                          <FastField
                            name="password"
                            type="password"
                            className="border-right-0 form-control"
                            component="input"
                            placeholder="Enter a password"
                          />
                          <div className="input-group-append">
                            <span className="input-group-text text-muted bg-transparent border-left-0">
                              <em className="fa fa-lock" />
                            </span>
                          </div>
                        </div>
                        {touched.password && errors.password && (
                          <div className="text-danger">{errors.password}</div>
                        )}
                      </div>
                      <div className="form-group">
                        <label
                          className="text-muted"
                          htmlFor="signupInputRePassword1"
                        >
                          Re-type Password
                        </label>
                        <div className="input-group with-focus">
                          <FastField
                            name="passwordConfirm"
                            type="password"
                            className="border-right-0 form-control"
                            component="input"
                            placeholder="Confirm the password"
                          />
                          <div className="input-group-append">
                            <span className="input-group-text text-muted bg-transparent border-left-0">
                              <em className="fa fa-lock" />
                            </span>
                          </div>
                        </div>
                        {touched.passwordConfirm && errors.passwordConfirm && (
                          <div className="text-danger">
                            {errors.passwordConfirm}
                          </div>
                        )}
                      </div>
                      <div className="form-group">
                        <div className="form-check accordion" id="accordion">
                          <div className="col-xs-6">
                            <label className="custom-radio">
                              <Field
                                type="radio"
                                name="userRole"
                                value={"User"}
                                className="form-check-input"
                              />
                              User{" "}
                            </label>
                            <span
                              id="toggleUser"
                              data-toggle="collapse"
                              data-target="#userInfo"
                              aria-expanded="false"
                              aria-controls="collapseOne"
                            >
                              <i className="ml-2 fa fa-question-circle"></i>
                            </span>
                            <div
                              id="userInfo"
                              className="collapse hide"
                              aria-labelledby="userInfo"
                              data-parent="#accordion"
                            >
                              <p>
                                As a User, you will have access to purchasing
                                products for sale on the ReCycle site. You will
                                have access to view events and the community
                                forum.
                              </p>
                            </div>
                          </div>
                          <div className="col-xs-6">
                            <label className="custom-radio">
                              <Field
                                type="radio"
                                name="userRole"
                                value={"Seller"}
                                className="form-check-input"
                              />
                              Seller
                            </label>
                            <span
                              id="toggleSeller"
                              data-toggle="collapse"
                              data-target="#sellerInfo"
                              aria-expanded="false"
                              aria-controls="collapseTwo"
                            >
                              <i className="ml-2 fa fa-question-circle"></i>
                            </span>
                            <div
                              id="sellerInfo"
                              className="collapse hide"
                              aria-labelledby="sellerInfo"
                              data-parent="#accordion"
                            >
                              <p>
                                As a Seller, you will have access to managing
                                and selling your inventory on the ReCycle site,
                                and take part in hosting events. For further
                                information on becoming a Seller, please visit
                                our <a href="/">FAQ page</a>.
                              </p>
                            </div>
                          </div>
                        </div>
                      </div>
                      <hr />
                      <div className="custom-checkbox custom-control">
                        <Field
                          type="checkbox"
                          name="terms"
                          id="terms"
                          className="custom-control-input"
                        />
                        <label className="custom-control-label" htmlFor="terms">
                          I agree with the Terms and Conditions
                        </label>
                      </div>
                      <button
                        className="btn btn-block btn-primary mt-3"
                        type="submit"
                      >
                        Create account
                      </button>
                    </div>
                  </div>
                  <div className="card-footer" style={footer}>
                    <div className="card-body">
                      <p className="text-center pb-2">Have an account?</p>
                      <button
                        className="btn btn-block btn-dark mb-3"
                        onClick={this.toLogin}
                      >
                        Sign-in
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </Form>
          )}
        />
      </>
    );
  }
}

Register.propTypes = {
  history: PropTypes.object.isRequired
};

export default Register;
