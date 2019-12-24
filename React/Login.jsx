import React from "react";
import logger from "sabio-debug";
import * as userService from "../../services/userService";
import { Formik, FastField, Form } from "formik";
import PropTypes from "prop-types";
import * as userSchema from "./userValidationSchema";
import Swal from "sweetalert";

const _logger = logger.extend("Login");

class Login extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      formData: {
        email: "",
        password: ""
      },
      user: {}
    };
  }

  handleSubmit = formValues => {
    this.setState(
      () => {
        return {
          formData: formValues
        };
      },
      () =>
        userService
          .login(formValues)
          .then(this.onLoginSuccess)
          .catch(this.onLoginError)
    );

    _logger(this.state.formData);
  };

  onLoginSuccess = () => {
    userService.checkAuth().then(this.onGetCurrentUserSuccess);
  };

  onLoginError = error => {
    _logger(error);
    Swal({
      title: "Something went wrong.",
      text:
        "Please verify that the email and password you have entered are correct.",
      type: "error",
      button: "Ok"
    });
  };

  onGetCurrentUserSuccess = response => {
    this.setState({ user: response.item });
    Swal({
      title: "Login Successful",
      text: "Welcome back!",
      type: "success",
      button: "OK",
      timer: 2500
    });
    if (response.item.roles.includes("Admin")) {
      this.props.history.push("/admin", {
        type: "login",
        user: response.item
      });
    } else if (response.item.roles.includes("Seller")) {
     
      this.props.history.push("/admin/seller/dashboard", {
        type: "login",
        user: response.item
      });
    } else {
     
      this.props.history.push("/");
    }
  };

  toRegister = () => {
    this.props.history.push("/register");
  };

  forgotPasswordClick = () => {
    this.props.history.push("/forgotPassword");
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
          validationSchema={userSchema.logSchema}
          enableReinitialize={true}
          render={formikProps => (
            <>
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
                    <p className="text-center py-2">SIGN IN TO CONTINUE.</p>
                    <Form className="mb-3" name="formLogin">
                      <div className="form-group">
                        <div className="input-group with-focus">
                          <FastField
                            name="email"
                            type="email"
                            className="border-right-0 form-control"
                            component="input"
                            placeholder="Enter your email"
                          />
                          <div className="input-group-append">
                            <span className="input-group-text text-muted bg-transparent border-left-0">
                              <em className="fa fa-envelope" />
                            </span>
                          </div>
                        </div>
                        {formikProps.touched.email &&
                          formikProps.errors.email && (
                            <div className="text-danger">
                              {formikProps.errors.email}
                            </div>
                          )}
                      </div>
                      <div className="form-group">
                        <div className="input-group with-focus">
                          <FastField
                            name="password"
                            type="password"
                            className="border-right-0 form-control"
                            component="input"
                            placeholder="Enter your password"
                          />
                          <div className="input-group-append">
                            <span className="input-group-text text-muted bg-transparent border-left-0">
                              <em className="fa fa-lock" />
                            </span>
                          </div>
                        </div>
                        {formikProps.touched.password &&
                          formikProps.errors.password && (
                            <div className="text-danger">
                              {formikProps.errors.password}
                            </div>
                          )}
                      </div>
                      <div className="clearfix">
                        <div className="checkbox c-checkbox float-left mt-0">
                          <label>
                            <input type="checkbox" name="remember" value="" />
                            <span className="fa fa-check" />
                            Remember Me
                          </label>
                        </div>
                        <div className="float-right">
                          <a
                            className="text-muted"
                            style={{ cursor: "pointer" }}
                            onClick={this.forgotPasswordClick}
                          >
                            Forgot your password?
                          </a>
                        </div>
                      </div>
                      <button
                        className="btn btn-block btn-primary mt-3"
                        type="submit"
                      >
                        Login
                      </button>
                    </Form>
                  </div>
                  <div className="card-footer" style={footer}>
                    <div className="card-body">
                      <p className="text-center pb-2">Need to Signup?</p>
                      <button
                        className="btn btn-block btn-dark"
                        onClick={this.toRegister}
                      >
                        Register Now
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </>
          )}
        />
      </>
    );
  }
}

Login.propTypes = {
  history: PropTypes.object.isRequired
};

export default Login;
