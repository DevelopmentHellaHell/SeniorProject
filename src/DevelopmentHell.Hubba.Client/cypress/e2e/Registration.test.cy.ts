/// <reference types="cypress" />

let url: string = 'http://localhost:3000/login';

describe('registration-pass', () => {
  beforeEach(()=> {
    cy.visit('http://localhost:3000/registration')
  })

  it('working-redirect-login-link', () => {
    //#element_id
    cy.get('#redirect-login').click()
    cy.url().should('eq',url)
  })

  it('registration-successful', () => {
    cy.get('#email')
      .type('registration@gmail.com')
      .should('have.value', 'registration@gmail.com')

    cy.get('#password')
    .type('12345678')
    .should('have.value', '12345678')

    cy.get('#confirm-password')
    .type('12345678')
    .should('have.value', '12345678')

    cy.get('.registration-card .buttons Button').click()
    cy.url().should('eq', url)
  })

})

describe('registration-fail-email-existed', () => {
  beforeEach(() => {
    cy.visit('http://localhost:3000/registration')
  })
  it('register-new-email', () => {
    cy.get('#email')
      .type('registration-duplicate@gmail.com')
      .should('have.value', 'registration-duplicate@gmail.com')

    cy.get('#password')
      .type('12345678')
      .should('have.value', '12345678')

    cy.get('#confirm-password')
      .type('12345678')
      .should('have.value', '12345678')

    cy.get('.registration-card .buttons Button').click()
    cy.url().should('eq', url)
  })

  it('register-again', () => {
    cy.get('#email')
      .type('registration-duplicate@gmail.com')
      .should('have.value', 'registration-duplicate@gmail.com')

    cy.get('#password')
      .type('12345678')
      .should('have.value', '12345678')

    cy.get('#confirm-password')
      .type('12345678')
      .should('have.value', '12345678')

    cy.get('.registration-card .buttons Button').click()
    cy.get('error').should('not.be.empty')
  })
})

describe('registration-fail-empty-input', () => {
  beforeEach(() => {
    cy.visit('http://localhost:3000/registration')
  })

  it('empty-email', () => {
    cy.get('#password')
      .type('12345678')
      .should('have.value', '12345678')

    cy.get('#confirm-password')
      .type('12345678')
      .should('have.value', '12345678')

    cy.get('.registration-card .buttons Button').click()
    cy.get('error').should('not.be.empty')
  })

  it('empty-password', () => {
    cy.get('#email')
      .type('registration@gmail.com')
      .should('have.value', 'registration@gmail.com')

    cy.get('.registration-card .buttons Button').click()
    cy.get('error').should('not.be.empty')
  })

  it('empty-confirm-password', () => {
    cy.get('#email')
      .type('registration@gmail.com')
      .should('have.value', 'registration@gmail.com')

    cy.get('#password')
    .type('12345679')
    .should('have.value', '12345679')

    cy.get('.registration-card .buttons Button').click()
    cy.get('error').should('not.be.empty')
  })
}) 

describe('registration-fail-passwords-unmatched', () => {
  beforeEach(() => {
    cy.visit('http://localhost:3000/registration')
  })

  it('type email, pw, confirmed pw, click Submit button, result Error message', () => {
    // https://on.cypress.io/type
    cy.get('#email')
      .type('registration@gmail.com')
      .should('have.value', 'registration@gmail.com')

    cy.get('#password')
      .type('12345678')
      .should('have.value', '12345678')

    cy.get('#confirm-password')
      .type('12345679')
      .should('have.value', '12345679')

    cy.get('.registration-card .buttons Button').click()
    cy.get('error').should('not.be.empty')
  })
}) 

describe('registration-fail-invalid-input', () => {
  beforeEach(() => {
    cy.visit('http://localhost:3000/registration')
  })
  it('invalid email', () => {
    cy.get('#email')
      .type('registrationgmail.com')
      .should('have.value', 'registrationgmail.com')

    cy.get('.registration-card .buttons Button').click()
    cy.get('error').should('not.be.empty')
  })

  it('invalid-password', () => {
    // https://on.cypress.io/type
    cy.get('#email')
      .type('registration@gmail.com')
      .should('have.value', 'registration@gmail.com')
    
    cy.get('#password')
    .type('1234567')
    .should('have.value', '1234567')

    cy.get('#confirm-password')
      .type('1234567')
      .should('have.value', '1234567')

    cy.get('.registration-card .buttons Button').click()
    cy.get('error').should('not.be.empty')
  })
})