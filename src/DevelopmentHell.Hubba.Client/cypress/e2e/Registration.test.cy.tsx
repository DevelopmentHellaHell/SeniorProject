/// <reference types="cypress" />
/* cy.get(), cy.contains()
** https://filiphric.com/cypress-basics-selecting-elements
** https://on.cypress.io/type
*/

import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";

let registrationUrl: string = 'http://localhost:3000/registration';
let loginUrl: string = 'http://localhost:3000/login';
let testsRoute: string = '/tests/deleteDatabaseRecords';

//describe.only to run a single test case
describe('working-links', () => {
  beforeEach(() => {
    cy.visit(registrationUrl);
  });

  it('working-sign-up-button', () => {
    //#element_id
    //cy.get('.nav-guest .buttons Button').click()
    cy.contains("Sign Up").click();
    cy.url().should('eq', registrationUrl);
  });

  it('working-login-button', () => {
    //#element_id
    //cy.get('.nav-guest .buttons Button').click()
    cy.contains("Login").click();
    cy.url().should('eq', loginUrl);
  });

  it('working-redirect-login-link', () => {
    //#element_id
    cy.get('#redirect-login').click();
    cy.url().should('eq', loginUrl);
  });
});

describe('registration-pass', () => {
  beforeEach(() => {
    cy.visit(registrationUrl);
  });

  afterEach(async () => {
    await Ajax.post(testsRoute, { database: Database.Databases.USERS });
  });

  it('registration-successful', () => {
    cy.get('#email')
      .type('registration@gmail.com')
      .should('have.value', 'registration@gmail.com');

    cy.get('#password')
      .type('12345678')
      .should('have.value', '12345678');

    cy.get('#confirm-password')
      .type('12345678')
      .should('have.value', '12345678');

    cy.contains("Submit").click();
    cy.url().should('eq', loginUrl);
  })

})

describe('registration-fail-email-existed', () => {
  beforeEach(() => {
    cy.visit(registrationUrl);
  });

  afterEach(async () => {
    await Ajax.post(testsRoute, { database: Databases.USERS });
  });

  it('register-new-email', () => {
    cy.get('#email')
      .type('registration-duplicate@gmail.com')
      .should('have.value', 'registration-duplicate@gmail.com');

    cy.get('#password')
      .type('12345678')
      .should('have.value', '12345678');

    cy.get('#confirm-password')
      .type('12345678')
      .should('have.value', '12345678');

    cy.contains("Submit").click();
    cy.url().should('eq', loginUrl);
  })

  it('register-again', () => {
    cy.get('#email')
      .type('registration-duplicate@gmail.com')
      .should('have.value', 'registration-duplicate@gmail.com');

    cy.get('#password')
      .type('12345678')
      .should('have.value', '12345678');

    cy.get('#confirm-password')
      .type('12345678')
      .should('have.value', '12345678');

    cy.contains("Submit").click();
    cy.get('error').should('not.be.empty');
  });
});

describe('registration-fail-invalid-input', () => {
  beforeEach(() => {
    cy.visit(registrationUrl);
  });

  afterEach(async () => {
    await Ajax.post(testsRoute, { database: Databases.USERS });
  });

  it('empty-email', () => {
    cy.get('#password')
      .type('12345678')
      .should('have.value', '12345678');

    cy.get('#confirm-password')
      .type('12345678')
      .should('have.value', '12345678');

    cy.contains("Submit").click();
    cy.get('error').should('not.be.empty');
  });

  it('empty-password', () => {
    cy.get('#email')
      .type('registration@gmail.com')
      .should('have.value', 'registration@gmail.com');

    cy.contains("Submit").click()
    cy.get('error').should('not.be.empty');
  })

  it('empty-confirm-password', () => {
    cy.get('#email')
      .type('registration@gmail.com')
      .should('have.value', 'registration@gmail.com');

    cy.get('#password')
      .type('12345679')
      .should('have.value', '12345679');

    cy.contains("Submit").click();
    cy.get('error').should('not.be.empty');
  })

  it('passwords-unmatched', () => {
    cy.get('#email')
      .type('registration@gmail.com')
      .should('have.value', 'registration@gmail.com');

    cy.get('#password')
      .type('12345678')
      .should('have.value', '12345678');

    cy.get('#confirm-password')
      .type('12345679')
      .should('have.value', '12345679');

    cy.contains("Submit").click();
    cy.get('error').should('not.be.empty');
  })

  it('invalid email', () => {
    cy.get('#email')
      .type('registrationgmail.com')
      .should('have.value', 'registrationgmail.com');

    cy.contains("Submit").click();
    cy.get('error').should('not.be.empty');
  });

  it('invalid-password', () => {
    cy.get('#email')
      .type('registration@gmail.com')
      .should('have.value', 'registration@gmail.com');

    cy.get('#password')
      .type('1234567')
      .should('have.value', '1234567');

    cy.get('#confirm-password')
      .type('1234567')
      .should('have.value', '1234567');

    cy.contains("Submit").click();
    cy.get('error').should('not.be.empty');
  });
});