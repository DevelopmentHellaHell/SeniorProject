import { defineConfig } from "cypress";

export default defineConfig({

  e2e: {
    baseUrl: "http://localhost:3000",
    setupNodeEvents(on, config) {
      // implement node event listeners here
    },
  },
  env:{
    baseUrl: "http://localhost:3000",
    serverUrl: "https://localhost:7137",

    realEmail: "kevin.lieu.dinh@gmail.com",
    standardEmail: "hubba@gmail.com",
    standardPassword: '12345678',
    realNumber: "5107755205"
  }
});
