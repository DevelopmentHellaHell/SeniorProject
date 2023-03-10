/// <reference types="cypress" />
export { }
declare global {
    namespace Cypress {
        interface Chainable {
            registerViaApi(emai: string, password: string): Chainable<void>
            loginViaApi(email: string, password: string): Chainable<void>
            //   drag(subject: string, options?: Partial<TypeOptions>): Chainable<Element>
            //   dismiss(subject: string, options?: Partial<TypeOptions>): Chainable<Element>
            //   visit(originalFn: CommandOriginalFn, url: string, options: Partial<VisitOptions>): Chainable<Element>
        }
    }
}
// ***********************************************
// This example commands.ts shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//

//
// -- This is a parent command --

Cypress.Commands.add('registerViaApi', (
    _email: string,
    _password: string) => {
    cy.request('POST', Cypress.env('registerApiUrl'), {
        email: _email,
        password: _password,
    }).its('status').should('eq', 200)
})

Cypress.Commands.add('loginViaApi', (_email, _password) => {
    cy.session([_email, _password], () => {
        cy.request('POST', Cypress.env('loginApiUrl'), {
            email: _email,
            password: _password
        }).its('status').should('eq', 200)
    })
    cy.request('GET', Cypress.env('getOtpApiUrl'))
        .then((response) => {
            cy.wrap(response.body).as('returnedOtp')
        })

    cy.get('@returnedOtp')
        .then((otp) => {
            cy.get('#otp').as('otp')
            cy.request('POST', 'authentication/otp', '@otp')
                .its('status').should('eq',200)
        })
    cy.visit(Cypress.env('baseUrl'))
})
//
//
// -- This is a child command --
// Cypress.Commands.add('drag', { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add('dismiss', { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite('visit', (originalFn, url, options) => { ... })
//

