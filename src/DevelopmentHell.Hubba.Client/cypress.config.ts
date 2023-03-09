import { defineConfig } from "cypress";

export default defineConfig({
  e2e: {
    baseUrl: "http://localhost:3000",
    setupNodeEvents(on, config) {
      // implement node event listeners here
    },
  },
  env:{
    baseUrl: "http://localhost:3000/",
    registrationUrl: "http://localhost:3000/registration",
    loginUrl: "http://localhost:3000/login"
  }
});
