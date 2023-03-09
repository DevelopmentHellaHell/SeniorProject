/// <reference types="cypress" />

/* cy.get(), cy.contains()
** https://filiphric.com/cypress-basics-selecting-elements
** https://on.cypress.io/type
*/

//describe.only to run a single test case

//User can't access dropdown menu before login
describe('unauthorized-user-check', () => {
  beforeEach(() => {
    cy.visit(Cypress.env('registrationUrl'))
  })
  it('can-only-access-navBar-for-guest', () => {
    cy.get('.nav-user').should('not.exist')
  })
  it('redirect-to-login-page-when-visit-account-by-broswer-bar', () => {
    cy.visit('/account')
      .then(()=>{
        cy.url().should('eq', Cypress.env('loginUrl'))
      })
  })
  it('show-unauthorized-message-when-visit-analytics-by-broswer-bar', () => {
    cy.visit('http://localhost:3000/analytics')
    cy.url().should('eq', 'http://localhost:3000/')
  })
})

describe('working-links', () => {
  beforeEach(() => {
    cy.visit(Cypress.env("registrationUrl"))
  })

  it('working-sign-up-button', () => {
    cy.contains("Sign Up").click()
    cy.url().should('eq', Cypress.env("registrationUrl"))
  })

  it('working-login-button', () => {
    cy.contains("Login").click()
    cy.url().should('eq', Cypress.env("loginUrl"))
  })

  it('working-redirect-login-link', () => {
    cy.get('#redirect-login').click()
    cy.url().should('eq', Cypress.env("loginUrl"))
  })
})

describe('registration-with-email-password', () => {
  beforeEach(function () {
    cy.visit(Cypress.env("registrationUrl"))
    //load standardEmail, dummyEmail, standardPassword, dummyPassword from /fixtures/credentials.json
    //then reuse in each it block
    //must use function() to wrap the fixture, not () =>
    cy.fixture('credentials.json')
      .then((credentials) => {
        this._credentials = credentials
      })

  })

  it('registration-successful-under-5s', function () {
    cy.get('#email')
      .type(this._credentials.tienEmail)
      .should('have.value', this._credentials.tienEmail)

    cy.get('#password')
      .type(this._credentials.standardPassword)
      .should('have.value', this._credentials.standardPassword)

    cy.get('#confirm-password')
      .type('12345678')
      .should('have.value', this._credentials.standardPassword)

    //timer
    const start: number = Date.now()
    cy.contains("Submit").click()
      .then(() => {
        cy.url().should('eq', Cypress.env('loginUrl'))
        const elapsed = Date.now() - start
        expect(elapsed).to.be.lessThan(5000)
      })
  })

  it('registration-fail-email-existed', function () {
    cy.get('#email')
      .type(this._credentials.standardEmail)
      .should('have.value', this._credentials.standardEmail)

    cy.get('#password')
      .type(this._credentials.standardPassword)
      .should('have.value', this._credentials.standardPassword)

    cy.get('#confirm-password')
      .type(this._credentials.standardPassword)
      .should('have.value', this._credentials.standardPassword)

    cy.contains("Submit").click()
      .then(() => {
        cy.get('error').should('not.be.empty')
      })
  })

  it('registration-fail-empty-email', function () {
    cy.get('#password')
      .type(this._credentials.standardPassword)
      .should('have.value', this._credentials.standardPassword)

    cy.get('#confirm-password')
      .type(this._credentials.standardPassword)
      .should('have.value', this._credentials.standardPassword)

    cy.contains("Submit").click()
      .then(() => {
        cy.get('error').should('not.be.empty')
      })
  })

  it('registration-fail-empty-password', function () {
    cy.get('#email')
      .type(this._credentials.standardEmail)
      .should('have.value', this._credentials.standardEmail)

    cy.contains("Submit").click()
      .then(() => {
        cy.get('error').should('not.be.empty')
      })
  })

  it('registration-fail-empty-confirm-password', function () {
    cy.get('#email')
      .type(this._credentials.standardEmail)
      .should('have.value', this._credentials.standardEmail)

    cy.get('#password')
      .type(this._credentials.standardPassword)
      .should('have.value', this._credentials.standardPassword)

    cy.contains("Submit").click()
      .then(() => {
        cy.get('error').should('not.be.empty')
      })
  })

  it('registration-fail-passwords-unmatched', function () {
    cy.get('#email')
      .type(this._credentials.standardEmail)
      .should('have.value', this._credentials.standardEmail)

    cy.get('#password')
      .type(this._credentials.standardPassword)
      .should('have.value', this._credentials.standardPassword)

    cy.get('#confirm-password')
      .type(this._credentials.dummyPassword)
      .should('have.value', this._credentials.dummyPassword)

    cy.contains("Submit").click()
      .then(() => {
        cy.get('error').should('not.be.empty')
      })
  })

  it('registration-fail-invalid email', function () {
    cy.get('#email')
      .type(this._credentials.dummyEmail)
      .should('have.value', this._credentials.dummyEmail)

    cy.contains("Submit").click()
      .then(() => {
        cy.get('error').should('not.be.empty')
      })
  })

  it('registration-fail-invalid-password', function () {
    cy.get('#email')
      .type(this._credentials.standardEmail)
      .should('have.value', this._credentials.standardEmail)

    cy.get('#password')
      .type(this._credentials.dummyPassword)
      .should('have.value', this._credentials.dummyPassword)

    cy.get('#confirm-password')
      .type(this._credentials.dummyPassword)
      .should('have.value', this._credentials.dummyPassword)

    cy.contains("Submit").click()
      .then(() => {
        cy.get('error').should('not.be.empty')
      })
  })
})
