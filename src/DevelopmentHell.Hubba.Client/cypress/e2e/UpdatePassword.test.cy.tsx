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

    it('user updates to new password', () => {
        cy.LoginViaApi(realEmail, standardPassword);
        cy.visit('/account');
        cy.contains("Login & Security").click();
        cy.contains('Update').click();

        //this was hard coded, please check to see if we can extract later
        cy.get('#otp').clear().type("K1ZFsdGc");
        cy.contains("Submit").click();

        cy.get('#oldPassword').clear().type(Cypress.env('standardPassword'));
        cy.get('#newPassword').clear().type(Cypress.env('alternativePassword'));
        cy.get('#newPasswordDupe').clear().type(Cypress.env('alternativePassword'));

        cy.contains("Submit").click();
        cy.wait(1000)
        //check to see if we return to Login & Security View
        cy.contains("Login");
        cy.contains("Account");
    });

    it('user cancels change password process', () => {
        cy.LoginViaApi(realEmail, standardPassword);
        cy.visit('/account');
        cy.contains("Login & Security").click();
        cy.contains('Update').click();

        //this was hard coded, please check to see if we can extract later
        cy.get('#otp').clear().type("K1ZFsdGc");
        cy.contains("Submit").click();
        cy.wait(1000);
        cy.contains("Change Password");


        //submitted without any input
        cy.contains("Submit").click();
        cy.get("p.error").contains("Please enter in your original password.");
        cy.wait(1000);

        //new passwords are duplicates of old one
        cy.get('#oldPassword').clear().type(Cypress.env('standardPassword'));
        cy.get('#newPassword').clear().type(Cypress.env('standardPassword'));
        cy.get('#newPasswordDupe').clear().type(Cypress.env('standardPassword'));
        cy.contains("Submit").click();
        cy.get("p.error").contains("You have entered the same password. Please try again.");
        cy.wait(1000);
       
        //new passwords do not match
        cy.get('#oldPassword').clear().type(Cypress.env('standardPassword'));
        cy.get('#newPassword').clear().type(Cypress.env('standardPassword'));
        cy.get('#newPasswordDupe').clear().type(Cypress.env('wrongPassword'));
        cy.contains("Submit").click();
        cy.get("p.error").contains("Please ensure that both entries for your new password match.");
        cy.wait(1000);
        

        //returns to Login & Security View
        cy.contains("Cancel").click();
    }); 

})



