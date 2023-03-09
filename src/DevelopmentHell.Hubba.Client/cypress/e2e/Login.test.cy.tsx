/// <reference types="cypress" />


//TODO
describe('unauthorized-user-check', () => {
    let baseUrl = Cypress.env('baseUrl')
    let loginUrl = Cypress.env('loginUrl')
    let registrationUrl = Cypress.env('registrationUrl')
    beforeEach(() => {
        cy.visit(loginUrl)
    })
    it('can-only-access-navBar-for-guest', () => {
        cy.get('.nav-user').should('not.exist')
    })
    it('redirect-to-login-page-when-visit-account-by-broswer-bar', () => {
        cy.visit('/account')
            .then(() => {
                cy.url().should('eq', loginUrl)
            })
    })
    it('redirect-to-home-page-when-visit-analytics-by-browser-bar', () => {
        cy.visit('/analytics')
        cy.url().should('eq', baseUrl)
    })
})

describe('working-links', () => {
    let baseUrl = Cypress.env('baseUrl')
    let loginUrl = Cypress.env('loginUrl')
    let registrationUrl = Cypress.env('registrationUrl')
    beforeEach(() => {
        cy.visit(loginUrl)
    })

    it('working-sign-up-button', () => {
        cy.contains("Sign Up").click()
        cy.url().should('eq', registrationUrl)
    })

    it('working-login-button', () => {
        cy.contains("Login").click()
        cy.url().should('eq', loginUrl)
    })

    it('working-redirect-registartion-link', () => {
        cy.get('#redirect-registration').click()
        cy.url().should('eq', registrationUrl)
    })

})

describe('login-successful', () => {
    let baseUrl = Cypress.env('baseUrl')
    let loginUrl = Cypress.env('loginUrl')
    let registrationUrl = Cypress.env('registrationUrl')
    beforeEach(function () {
        cy.visit(loginUrl)
        cy.fixture('/credentials.json')
            .then((credentials) => {
                this._credentials = credentials;
            })
    })

    it('login-successful-pass-otp-check', function () {
        cy.get('#email')
            .type(this._credentials.tienEmail)
            .should('have.value', this._credentials.tienEmail)
        cy.get('#password')
            .type(this._credentials.standardPassword)
            .should('have.value', this._credentials.standardPassword)
        cy.contains('Submit').click()
            .then(() => {
                cy.get('.otp-card')
                    .should('exist').and('be.visible')
                let returnedOtp
                cy.request('GET', 'https://localhost:7137/tests/getotp')
                    .then((response) => {
                        cy.wrap(response.body).as('returnedOtp')
                    })
                cy.get('@returnedOtp')
                    .then((otp) => {
                        cy.get('#otp')
                            .type(otp)
                            .should('have.value', otp)
                    })
                cy.contains('Submit').click()
                    .then(()=>{
                        cy.url().should('eq', baseUrl)
                        cy.get('.nav-user').should('exist').and('be.visible')
                    })
            })
    })
})