export default class Utils {
  static handleAxiosError(error) {
    if (error.response) {
      console.log(error.response.data);
      console.log(error.response.status);
      console.log(error.response.headers);
    } else if (error.request) {
      console.log(error.request);
    } else {
      console.log("Error", error.message);
    }
    console.log(error.config);
    return "Error: " + error.message;
  }

  static nameHandler(input) {
    let invalid = true;
    let message = null;
    if (input.length > 15) {
      message = "Cannot be longer than 15 characters";
      invalid = true;
    } else if (/[\\/:*?"<>|]/.test(input)) {
      message = 'Cannot contain these characters: \\ / : * ? " < > |';
      invalid = true;
    } else if (input.startsWith(".")) {
      message = "Cannot start with ' . '";
      invalid = true;
    } else {
      invalid = false;
    }
    return { invalid: invalid, message: message };
  }

  static ipHandler(input) {
    const ipPattern = /^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/;
    if (!ipPattern.test(input)) {
      return true;
    } else {
      return false;
    }
  }

  static simulateLoading() {
    return new Promise((resolve) => setTimeout(resolve, 1000));
  }
}
