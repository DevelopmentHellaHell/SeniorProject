/// <reference types="cypress" />
import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";
describe('test', () => {
    let baseUrl: string = Cypress.env('baseUrl') + "/";
    let loginUrl: string = Cypress.env('baseUrl') + "/login";
    let realEmail: string = Cypress.env("realEmail");
    let standardPassword: string = Cypress.env("standardPassword");
    let testsRoute: string = '/tests/deleteDatabaseRecords';

    before(() => {
        Cypress.session.clearAllSavedSessions();
        //using Custom Commands
        cy.RegisterViaApi(Cypress.env('realEmail'), Cypress.env('standardPassword'));
        cy.visit('/account-recovery');
    });
    

    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
    })

    it('account recovery adds to recovery requests', () => {
        //force show the dropdown menu

        cy.get('#email').as('email').type(realEmail).should('have.value', realEmail);
        cy.contains('Submit').click()
                .then(() => {
                    //valid email, password, show OTP card
                    cy.get('#otp-input').should('exist').and('be.visible');
                });
        //get OTP from the database
        cy.request('GET', Cypress.env('serverUrl') + "/tests/getotp")
        .then((response) => {
            cy.wrap(response.body).as('returnedOtp');
            cy.get('@returnedOtp')
                .then((otpString) => {
                    let otp = otpString.toString();
                    cy.get('#otp-input').type(otp).should('have.value', otp);
                    cy.contains('Submit').click();
                    cy.url().should('eq', baseUrl);
                    cy.get('.nav-guest').should('exist').and('be.visible');
                    cy.contains('Sign Up').should('exist');
                    cy.contains('Login').should('exist');
                });
        });
    });

    it('account recovery goes to immediate authentication', () => {
        //force show the dropdown menu
        cy.LoginViaApi(Cypress.env('realEmail'), Cypress.env('standardPassword'));
        cy.LogInandOut();
        cy.visit('/account-recovery');

        cy.get('#email').as('email').type(realEmail).should('have.value', realEmail);
        cy.contains('Submit').click()
                .then(() => {
                    //valid email, password, show OTP card
                    cy.get('#otp-input').should('exist').and('be.visible');
                });
        //get OTP from the database
        cy.request('GET', Cypress.env('serverUrl') + "/tests/getotp")
        .then((response) => {
            cy.wrap(response.body).as('returnedOtp');
            cy.get('@returnedOtp')
                .then((otpString) => {
                    let otp = otpString.toString();
                    cy.get('#otp-input').type(otp).should('have.value', otp);
                    cy.contains('Submit').click();
                    cy.url().should('eq', baseUrl + 'account');
                    cy.get('.nav-user').should('exist').and('be.visible');
                });
        });
    });

});