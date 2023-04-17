/// <reference types="cypress" />
export { }
declare global {
    namespace Cypress {
        interface Chainable {
            RegisterViaApi(emai: string, password: string): Chainable<void>;
            LoginViaUI(email: string, password: string): Chainable<void>;
            LoginViaApi(email: string, password: string): Chainable<void>;
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
 * Login by direct AJAX HTTP POST to API
 * @param: email, password
 */
Cypress.Commands.add('LoginViaApi', (email: string, password: string) => {
    cy.session([email, password], () => {
        cy.request('POST', Cypress.env('serverUrl') + "/authentication/login", { email, password })
            .then(() => {
                cy.request('GET', Cypress.env('serverUrl') + "/tests/getotp")
                    .then((response) => {
                        cy.wrap(response.body).as('returnedOtp');
                        cy.get('@returnedOtp')
                            .then((otpString) => {
                                let otp = otpString.toString();
                                cy.request('POST', Cypress.env('serverUrl')+'/authentication/otp', {otp});
                            })
                    });
            })
        })
})
        /**
         * Login with email and password via UI
         * Valid email, password will enable OTP input
         * Get OTP by direct AJAX HTTP GET to API via tests route
         * @param: email, password
         */
        Cypress.Commands.add('LoginViaUI', (email: string, password: string) => {
            cy.session([email, password], () => {
                cy.visit("/login");
                let startTimer: number;
                cy.get('#email').as('email').type(Cypress.env('realEmail')).should('have.value', Cypress.env('realEmail'));
                cy.get('#password').as('password').type(Cypress.env('standardPassword')).should('have.value', Cypress.env('standardPassword'));
                cy.contains('Submit').click()
                    .then(() => {
                        //valid email, password, show OTP card
                        cy.get('.otp-card').should('exist').and('be.visible');
                    })

                //get OTP from the database
                cy.request('GET', Cypress.env('serverUrl') + "/tests/getotp")
                    .then((response) => {
                        cy.wrap(response.body).as('returnedOtp');
                        cy.get('@returnedOtp')
                            .then((otpString) => {
                                let otp = otpString.toString();
                                cy.get('#otp').type(otp).should('have.value', otp);
                                startTimer = Date.now();
                                // time the login operation            
                                cy.contains('Submit').click()
                                    .then(() => {
                                        let endTimer: number = Date.now() - startTimer;
                                        expect(endTimer).lessThan(5000);
                                    });
                                cy.url().should('eq', Cypress.env('baseUrl') + '/');
                                cy.get('.nav-user').should('exist').and('be.visible');
                                cy.contains('Sign Up').should('not.exist');
                                cy.contains('Login').should('not.exist');
                            });
                    });
            });
        })
