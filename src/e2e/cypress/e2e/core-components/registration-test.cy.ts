/// <reference types="cypress" />

// context('login-page', () => {
//     beforeEach(() => {
//       cy.visit('http://localhost:3000/login')
//     })
  
//     // https://on.cypress.io/interacting-with-elements
  
//     it('.type() - type email, pw, confirmed pw a DOM element', () => {
//       // https://on.cypress.io/type
//       cy.get('input')
//         .type('tien.nguyen10@student.csulb.edu').should('have.value', 'tien.nguyen10@student.csulb.edu', { delay: 100 })
//     })
// })
let url: string = 'http://localhost:3000/login';
describe('e2e-registration-page-successful', () => {
  beforeEach(() => {
    cy.visit('http://localhost:3000/registration')
  })
  it('.type() - type email, pw, confirmed pw a DOM element', () => {
    // https://on.cypress.io/type
    cy.get('input').eq(0)
      .type('e2eregistrationpage@gmail.com')
      .should('have.value', 'e2eregistrationpage@gmail.com', { delay: 100 })

    cy.get('input').eq(1)
    .type('12345678')
    .should('have.value', '12345678', { delay: 100 })

    cy.get('input').eq(2)
    .type('12345678')
    .should('have.value', '12345678', { delay: 100 })

    cy.get('.registration-card .buttons Button').click()
    cy.url().should('eq', url)
  })
}) 

describe('e2e-registration-page-email-existed', () => {
  beforeEach(() => {
    cy.visit('http://localhost:3000/registration')
  })
  it('.type() - type email, pw, confirmed pw a DOM element', () => {
    // https://on.cypress.io/type
    cy.get('input').eq(0)
      .type('e2eregistrationpage@gmail.com')
      .should('have.value', 'e2eregistrationpage@gmail.com', { delay: 100 })

    cy.get('input').eq(1)
    .type('12345678')
    .should('have.value', '12345678', { delay: 100 })

    cy.get('input').eq(2)
    .type('12345678')
    .should('have.value', '12345678', { delay: 100 })

    cy.get('.registration-card .buttons Button').click()
    cy.get('error').should('not.be.empty')
  })
}) 