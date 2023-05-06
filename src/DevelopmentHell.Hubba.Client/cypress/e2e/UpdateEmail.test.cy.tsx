/// <reference types="cypress" />
import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";

/**
 * Test Notification functionality
 * User register, login, change Notification Settings, Notification Menu
 * Delete test data from database after testing
 */
describe('Navigate to Notification Settings', () => {
    let baseUrl: string = Cypress.env('baseUrl') + "/";
    let loginUrl: string = Cypress.env('baseUrl') + "/login";
    let realEmail: string = Cypress.env("realEmail");
    let standardPassword: string = Cypress.env("standardPassword");
    let testsRoute: string = '/tests/deleteDatabaseRecords';

    before(()=>{
        //clear all sessions include the backend and cache
        Cypress.session.clearAllSavedSessions();
        cy.RegisterViaApi(Cypress.env('realEmail'), Cypress.env('standardPassword'));
    })

    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
    });



    it('Update Email', () => {
        cy.LoginViaApi(realEmail, standardPassword);
        cy.visit('/account');
        cy.contains("Edit Profile");
        cy.contains("Change Email");
        cy.contains('Update').click();

        cy.get('#otp').clear().type("K1ZFsdGc");
        cy.contains("Submit").click();

        cy.get('#newEmail').clear().type(Cypress.env('standardEmail'));
        cy.get('#password').clear().type(Cypress.env('standardPassword'));
        

        cy.contains("Submit").click();

        cy.contains("Login");
    });

    before(()=>{
        //clear all sessions include the backend and cache
        Cypress.session.clearAllSavedSessions();
        cy.RegisterViaApi(Cypress.env('standardEmail'), Cypress.env('standardPassword'))
    });
    
    it('Update Email Errors', () => {
        cy.LoginViaApi(realEmail, standardPassword);
        cy.visit('/account');
        cy.contains("Edit Profile");
        cy.contains("Change Email");
        cy.contains('Update').click();

        cy.get('#otp').clear().type("K1ZFsdGc");
        cy.contains("Submit").click();

        //typed in duplicate email
        cy.get('#newEmail').clear().type(Cypress.env("realEmail"));
        cy.get('#password').clear().type(Cypress.env('standardPassword'));
        cy.contains("Submit").click();
        cy.get("p.error").contains("You need to enter a different email or press cancel.")
        cy.pause()
        cy.wait(1000);

        //typed in already existing email
        cy.get('#newEmail').clear().type(Cypress.env('standardEmail'));
        cy.contains("Submit").click();
        cy.get("p.error").contains("An email is already registered with this account. ");

        cy.wait(1000);

        //typed in wrong password
        cy.get('#password').clear().type(Cypress.env('wrongPassword'));
        cy.pause()
        cy.contains("Cancel").click();
    });
})