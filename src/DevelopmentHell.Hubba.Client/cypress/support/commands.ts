/// <reference types="cypress" />
export { }
declare global {
    namespace Cypress {
        interface Chainable {
            registerViaApi(emai: string, password: string): Chainable<void>
            loginViaUI(email: string, password: string): Chainable<void>
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

Cypress.Commands.add('registerViaApi', (email: string, password: string) => {
    cy.request('POST', Cypress.env('serverUrl')+"/registration/register", {email,password})
        .its('status').should('eq', 200);
})

Cypress.Commands.add('loginViaUI', (email:string, password:string) => {
    cy.session([email, password], () => {
        cy.visit('/login');
        cy.get('#email').type(Cypress.env('realEmail')).should('have.value', Cypress.env('realEmail'));
        cy.get('#password').type(Cypress.env('standardPassword')).should('have.value', Cypress.env('standardPassword'));
        cy.contains('Submit').click()
            .then(() => {
                cy.get('.otp-card')
                    .should('exist').and('be.visible');
                    cy.request('GET', Cypress.env('serverUrl')+"/tests/getotp")
                    .then((response) => {
                        cy.wrap(response.body).as('returnedOtp');
                    });
                
                cy.get('@returnedOtp')
                    .then((otp) => {
                        cy.get('#otp')
                            .type(otp)
                            .should('have.value', otp);
                    });
                cy.contains('Submit').click()
                    .then(()=>{
                        cy.url().should('eq', Cypress.env('baseUrl')+'/');
                        cy.get('.nav-user').should('exist').and('be.visible')
                    });
            });
    })
})