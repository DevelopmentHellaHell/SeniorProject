/// <reference types="cypress" />
export { }
declare global {
    namespace Cypress {
        interface Chainable {
            RegisterViaApi(emai: string, password: string): Chainable<void>
            LoginViaUI(email: string, password: string): Chainable<void>
        }
    }
}
/**
 * Register new account by direct AJAX HTTP POST to API
 * @param: email, password
 */
Cypress.Commands.add('RegisterViaApi', (email: string, password: string) => {
    cy.request('POST', Cypress.env('serverUrl') + "/registration/register", { email, password })
        .its('status').should('eq', 200);
})

/**
 * Login with email and password via UI
 * Valid email, password will enable OTP input
 * Get OTP by direct AJAX HTTP GET to API via tests route
 * @param: email, password
 */
Cypress.Commands.add('LoginViaUI', (email: string, password: string) => {
    cy.session([email, password], () => {
        cy.visit('/login');
        cy.get('#email').type(email).should('have.value', email);
        cy.get('#password').type(password).should('have.value', password);
        cy.contains('Submit').click()
            .then(() => {
                cy.get('.otp-card')
                    .should('exist').and('be.visible');
                cy.request('GET', Cypress.env('serverUrl') + "/tests/getotp")
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
                    .then(() => {
                        cy.url().should('eq', Cypress.env('baseUrl') + '/');
                        cy.get('.nav-user').should('exist').and('be.visible')
                    });
            });
    })
})