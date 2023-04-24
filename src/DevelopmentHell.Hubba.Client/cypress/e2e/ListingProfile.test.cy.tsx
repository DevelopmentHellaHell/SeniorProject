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
    });
    

    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
    })

    it('goes to create a listing', () => {
        //force show the dropdown menu
        cy.LoginViaApi(Cypress.env('realEmail'), Cypress.env('standardPassword'));
        cy.visit('/listingprofile');
        cy.get('.nav-user').should('exist').and('be.visible');

        cy.contains('Create Listing').click().then(() => {
            cy.get("title-input-field").should('exist').and('be.visible');
            cy.get("title-input-field").type("Automated testing 1")

        });
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
                    
                });
        });
    });

});