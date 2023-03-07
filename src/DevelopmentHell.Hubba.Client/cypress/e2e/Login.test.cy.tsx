/// <reference types="cypress" />
//TODO

describe('login-fail-invalid-input', () => {
    beforeEach(() => {
        cy.visit(Cypress.env('loginUrl'))
        cy.fixture('/credentials')
        .then((myCredentials) => {
            this.credentials = myCredentials
        })
    })

    it('empty-email', () => {
        cy.get('#password')
            .type(this.credentials.standardPassword)
            .should('have.value', this.credentials.standardPassword)

        cy.contains("Submit").click()
        cy.get('error').should('not.be.empty')
    })

    it('empty-password', () => {
        cy.get('#email')
            .type(this.credentials.standardEmail)
            .should('have.value', this.credentials.standardEmail)

        cy.contains("Submit").click()
        cy.get('error').should('not.be.empty')
    })

    it('invalid email', () => {
        cy.get('#email')
            .type(this.credentials.dummyEmail)
            .should('have.value', this.credentials.dummyEmail)

        cy.contains("Submit").click()
        cy.get('error').should('not.be.empty')
    })

    it('invalid-password', () => {
        cy.get('#email')
            .type(this.credentials.standardEmail)
            .should('have.value', this.credentials.standardEmail)

        cy.get('#password')
            .type(this.credentials.dummyPassword)
            .should('have.value', this.credentials.dummyPassword)

        cy.contains("Submit").click()
        cy.get('error').should('not.be.empty')
    })
})

describe.only('login-pass', () => {
    beforeEach(() => {
        cy.visit(Cypress.env("loginUrl"))
        cy.fixture('/credentials')
        .then((myCredentials) => {
            this._credentials = myCredentials
        })
    })

    it('login-successful-under-5s', () => {

        cy.get('#email')
            .type(this._credentials.tienEmail)
            .should('have.value', this._credentials.tienEmail);
        cy.get('#password')
            .type(this._credentials.tienPassword)
            .should('have.value', this._credentials.tienPassword)

        cy.get('.login-card .buttons Button').click()
            .should('have.property', 'status', 200) //successful AJAX HTTP POST request
        
        cy.get('.otp-card')
            .should('exist')
            .should('be.visible')
        cy.getCookie('access_token')
            .then((myCookie) => {
                cy.log('my cookie= ', myCookie)
            })

    })
})
