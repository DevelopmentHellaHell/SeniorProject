/// <reference types="cypress" />
import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";

describe('Tests involving Update Password', () => {
    let baseUrl: string = Cypress.env('baseUrl') + "/";
    let loginUrl: string = Cypress.env('baseUrl') + "/login";
    let realEmail: string = Cypress.env("realEmail");
    let standardPassword: string = Cypress.env("standardPassword");
    let testsRoute: string = '/tests/deleteDatabaseRecords';

    before(()=>{
        //clear all sessions include the backend and cache
        Cypress.session.clearAllSavedSessions();
        cy.RegisterViaApi(Cypress.env('realEmail'), Cypress.env('standardPassword'));
    });

    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
    });

    it('User updates first name last name from base null values', () => {
        cy.LoginViaApi(realEmail, standardPassword);
        cy.visit('/account');
        cy.contains("Edit Profile").click();
        cy.wait(1000);

        //Update first name and last name
        cy.get('#firstName').clear().type("Kevin");
        cy.get('#lastName').clear().type("Dinh");
        cy.contains("Save").click();
        cy.get("p.error").contains("Your username has been saved.");
    })

    it('User updates new first name after intial previously saved name', () => {
        cy.LoginViaApi(realEmail, standardPassword);
        cy.visit('/account');
        cy.contains("Edit Profile").click();
        cy.wait(1000);

        //Update first name and last name
        cy.get('#firstName').clear().type("Kevin");
        cy.get('#lastName').clear().type("Dinh");
        cy.contains("Save").click();
        cy.get("p.error").contains("Your username has been saved.");
        
        //Refresh Page
        cy.contains("Login & Security").click();
        cy.contains("Edit Profile").click();

        //Update first name with new value
        cy.get('#firstName').clear().type("Devin");
        cy.contains("Save").click();
        cy.get("p.error").contains("Your username has been saved.");
    })

    it('User updates new last name after intial previously saved name', () => {
        cy.LoginViaApi(realEmail, standardPassword);
        cy.visit('/account');
        cy.contains("Edit Profile").click();
        cy.wait(1000);

        //Update first name and last name
        cy.get('#firstName').clear().type("Kevin");
        cy.get('#lastName').clear().type("Dinh");
        cy.contains("Save").click();
        cy.get("p.error").contains("Your username has been saved.");
        
        //Refresh Page
        cy.contains("Login & Security").click();
        cy.contains("Edit Profile").click();

        //Update first name with new value
        cy.get('#lastName').clear().type("Kinh");
        cy.contains("Save").click();
        cy.get("p.error").contains("Your username has been saved.");
    })

    it('User clicks save without entering any values', () => {
        cy.LoginViaApi(realEmail, standardPassword);
        cy.visit('/account');
        cy.contains("Edit Profile").click();
        cy.wait(1000);

        //User clicks save 
        cy.contains("Save").click();
        cy.get("p.error").contains("Please enter in a name before saving.");
    })

})