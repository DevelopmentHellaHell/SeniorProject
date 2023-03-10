/// <reference types="cypress" />
import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";

// cy.get(), cy.contains()
// https://filiphric.com/cypress-basics-selecting-elements
// https://on.cypress.io/type


//describe.only to run a single test case
/**
 * Unauthenticated user restriction on the web app
 * User can only navigate to LoginPage, RegistrationPage, HomePage, DiscoverPage
 */
describe('unauthenticated user can only see certains pages', () => {
  beforeEach(() => {
    cy.visit(Cypress.env('baseUrl') + "/registration");
  })
  it('only access to Guest NavBar', () => {
    cy.get('.nav-user').should('not.exist');
  })
  it('redirect to LoginPage when visiting VerifiedUser-only page(s) by the browser bar', () => {
    cy.visit('/account')
      .then(() => {
        cy.url().should('eq', Cypress.env('baseUrl') + '/login');
      });
  })
  it('redirect to LoginPage when visiting AdminUser-only page(s) by the browser bar', () => {
    cy.visit('/analytics');
    cy.url().should('eq', Cypress.env('baseUrl') + '/');
  })
})

/**
 * Test all links and buttons on the page
 * Logo, NavBar links, NavBar buttons, refirect link
 */
describe('check working links', () => {
  let baseUrl = Cypress.env('baseUrl') + "/";
  let loginUrl = Cypress.env('baseUrl') + "/login";
  let registrationUrl = Cypress.env("baseUrl") + "/registration";
  // let realEmail = Cypress.env("realEmail");
  // let standardEmail = Cypress.env("standardEmail");
  // let standardPassword = Cypress.env("standardPassword");

  beforeEach(() => {
    cy.visit('/registration');
  })

  it('NavBar-Sign Up button', () => {
    cy.contains("Sign Up").click();
    cy.url().should('eq', registrationUrl);
  })

  it('NavBar-Login button', () => {
    cy.contains("Login").click();
    cy.url().should('eq', loginUrl);
  })

  it('RegistrationCard-Login redirect link', () => {
    cy.get('#redirect-login').click();
    cy.url().should('eq', loginUrl);
  })
})

/**
 * Register successfully with a valid email and password
 * After clicking Submit button, system responses under 5s
 */
describe('registration successful case', () => {
  let baseUrl = Cypress.env('baseUrl') + "/";
  let loginUrl = Cypress.env('baseUrl') + "/login";
  let registrationUrl = Cypress.env("baseUrl") + "/registration";
  // let realEmail = Cypress.env("realEmail");
  let standardEmail = Cypress.env("standardEmail");
  let standardPassword = Cypress.env("standardPassword");

  beforeEach(() => {
    cy.visit(registrationUrl);
  })

  it('register successfully with valid email, password under-5s', () => {
    cy.get('#email')
      .type(standardEmail)
      .should('have.value', standardEmail);

    cy.get('#password')
      .type(standardPassword)
      .should('have.value', standardPassword);

    cy.get('#confirm-password')
      .type(standardPassword)
      .should('have.value', standardPassword);

    //timer
    const start: number = Date.now()
    cy.contains("Submit").click()
      .then(() => {
        cy.url().should('eq', loginUrl);
        const elapsed = Date.now() - start;
        expect(elapsed).to.be.lessThan(5000);
      })
  })

})

/**
 * Register failed
 * test: validate input syntax before sending to the API
 * test: email existed
 */
describe('registration failed cases', () => {
  let baseUrl = Cypress.env('baseUrl') + "/";
  let loginUrl = Cypress.env('baseUrl') + "/login";
  let registrationUrl = Cypress.env("baseUrl") + "/registration";
  // let realEmail = Cypress.env("realEmail");
  let standardEmail = Cypress.env("standardEmail");
  let standardPassword = Cypress.env("standardPassword");
  let testsRoute: string = '/tests/deleteDatabaseRecords';

  beforeEach(() => {
    cy.visit(registrationUrl);
  })

  /**
   * Delete test cases from database after the test
   */
  after(async () => {
    await Ajax.post(testsRoute, { database: Database.Databases.USERS });
  });
  
  it('email existed', () => {
    cy.get('#email')
      .type(standardEmail)
      .should('have.value', standardEmail);

    cy.get('#password')
      .type(standardPassword)
      .should('have.value', standardPassword);

    cy.get('#confirm-password')
      .type(standardPassword)
      .should('have.value', standardPassword);

    cy.contains("Submit").click()
      .then(() => {
        cy.get('.registration-card .error')
          .should('exist').and('be.visible');
      });
  })

  it('empty email input', () => {
    cy.get('#password')
      .type(standardPassword)
      .should('have.value', standardPassword);

    cy.get('#confirm-password')
      .type(standardPassword)
      .should('have.value', standardPassword);

    cy.contains("Submit").click()
      .then(() => {
        cy.get('.registration-card .error')
          .should('exist').and('be.visible');
      });
  })

  it('empty password input', () => {
    cy.get('#email')
      .type(standardEmail)
      .should('have.value', standardEmail);

    cy.contains("Submit").click()
      .then(() => {
        cy.get('.registration-card .error')
          .should('exist').and('be.visible');
      });
  })

  it('empty confirm password input', () => {
    cy.get('#email')
      .type(standardEmail)
      .should('have.value', standardEmail);

    cy.get('#password')
      .type(standardPassword)
      .should('have.value', standardPassword);

    cy.contains("Submit").click()
      .then(() => {
        cy.get('.registration-card .error')
          .should('exist').and('be.visible');
      });
  })

  it('passwords input not match', () => {
    cy.get('#email')
      .type(standardEmail)
      .should('have.value', standardEmail);

    cy.get('#password')
      .type(standardPassword)
      .should('have.value', standardPassword);

    cy.get('#confirm-password')
      .type('12345679')
      .should('have.value', '12345679');

    cy.contains("Submit").click()
      .then(() => {
        cy.get('.registration-card .error')
          .should('exist').and('be.visible');
      });
  })

  it('invalid email', () => {
    cy.get('#email')
      .type('hello.com')
      .should('have.value', 'hello.com');

    cy.contains("Submit").click()
      .then(() => {
        cy.get('.registration-card .error')
          .should('exist').and('be.visible');
      });
  })

  it('invalid password', () => {
    cy.get('#email')
      .type(standardEmail)
      .should('have.value', standardEmail);

    cy.get('#password')
      .type('123456')
      .should('have.value', '123456');

    cy.get('#confirm-password')
      .type('123456')
      .should('have.value', '123456');

    cy.contains("Submit").click()
      .then(() => {
        cy.get('.registration-card .error')
          .should('exist').and('be.visible');
      })
  })
})
