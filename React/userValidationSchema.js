import * as Yup from "yup";

let regSchema = Yup.object().shape({
  email: Yup.string()
    .email("The email address entered is invalid.")
    .required("An Email is Required."),
  password: Yup.string()
    .min(8, "Password must be more than 8 characters.")
    .max(15, "Passwords cannot be longer 15 characters.")
    .matches(
      /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]/,
      "Must Contain 8 Characters, One Uppercase, One Lowercase, One Number and one special case Character"
    )
    .required("A password is required."),
  passwordConfirm: Yup.string()
    .min(8, "Password must be more than 8 characters.")
    .max(15, "Passwords cannot be longer than 15 characters.")
    .oneOf(
      [Yup.ref("password"), null],
      "Passwords must match, please re-enter the password."
    )
    .required("Password confirmation is required."),
  terms: Yup.bool()
    .required(
      "You must agree to the Terms & Conditions to register an account."
    )
    .oneOf(
      [true],
      "You must agree to the Terms & Conditions to register an account."
    )
});

let passwordSchema = Yup.object().shape({
  token: Yup.string().required("A token is required."),
  password: Yup.string()
    .min(8, "Password must be more than 8 characters.")
    .max(15, "Passwords cannot be longer 15 characters.")
    .matches(
      /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]/,
      "Must Contain 8 Characters, One Uppercase, One Lowercase, One Number and one special case Character"
    )
    .required("A password is required."),
  passwordConfirm: Yup.string()
    .min(8, "Password must be more than 8 characters.")
    .max(15, "Passwords cannot be longer than 15 characters.")
    .oneOf(
      [Yup.ref("password"), null],
      "Passwords must match, please re-enter the password."
    )
});

let logSchema = Yup.object().shape({
  email: Yup.string()
    .email("Invalid email address.")
    .required("An email address is required."),
  password: Yup.string().required("A password is required.")
});

export { regSchema, logSchema, passwordSchema };
