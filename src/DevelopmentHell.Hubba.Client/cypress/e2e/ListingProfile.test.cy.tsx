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


    it('create a listing', () => {
        cy.LoginViaApi(realEmail, standardPassword);
        cy.visit('/listingprofile');
        cy.get('.nav-user').should('exist').and('be.visible');
        cy.contains('You have no listings');
        cy.contains('Create Listing').click().then(() => {
            cy.get("#title-input").should('exist').and('be.visible');
            cy.get("#title-input").type("Automated testing").should('have.value', "Automated testing");
            cy.contains('Submit').click();
        });
        !cy.contains('You have no listings');
    });

    it('failure duplicate title a listing', () => {
        cy.LoginViaApi(realEmail, standardPassword);
        cy.visit('/listingprofile');
        cy.get('.nav-user').should('exist').and('be.visible');
        cy.contains('Create Listing').click().then(() => {
            cy.get("#title-input").should('exist').and('be.visible');
            cy.get("#title-input").type("Automated testing").should('have.value', "Automated testing");
            cy.contains('Submit').click();
        });
        cy.on('window:alert',(t)=>{
            //assertions
            expect(t).to.contains('Cannot create multiple listings with the same title');
         })
    });

    it('view a listing draft', () => {
        cy.LoginViaApi(realEmail, standardPassword);
        cy.visit('/listingprofile');
        cy.get('.nav-user').should('exist').and('be.visible');
        cy.contains('View').click().then(() => {
            cy.contains('Draft');
            cy.contains('Edit Listing');
            cy.contains('Delete Listing');
            cy.contains('Automated testing');
            cy.contains('Location:');
            cy.contains('View Ratings');
        })
    });

    it('edit a listing draft', () => {
        cy.LoginViaApi(realEmail, standardPassword);
        cy.visit('/listingprofile');
        cy.contains('View').click().then(() => {
            cy.contains('Draft');
            cy.contains('Edit Listing');
            cy.contains('Delete Listing');
            cy.contains('Automated testing');
            cy.contains('Location:');
            cy.contains('View Ratings');
        })
        cy.contains('Edit Listing').click().then(() => {
            cy.get("#price-input").type("232").should('have.value', '232');
            cy.get("#description-input").type("Test description").should('have.value', 'Test description');
            cy.contains('Save Changes').click();
            cy.contains('Draft');
            cy.contains('Edit Listing');
            cy.contains('Price: 232');
            cy.contains('Description: Test description');
        })
        
    });

    it('invalid description editting a listing draft', () => {
        cy.LoginViaApi(realEmail, standardPassword);
        cy.visit('/listingprofile');
        cy.contains('View').click().then(() => {
            cy.contains('Draft');
            cy.contains('Edit Listing');
            cy.contains('Delete Listing');
            cy.contains('Automated testing');
            cy.contains('Location:');
            cy.contains('View Ratings');
        })
        cy.contains('Edit Listing').click().then(() => {
            cy.get("#price-input").clear().should('have.value', '');
            cy.get("#description-input").type("%%%").should('have.value', 'Test description%%%');
            cy.contains('Save Changes').click();
            cy.on('window:alert',(t)=>{
                //assertions
                expect(t).to.contains('Invalid characters');
                true
             })
        })
    });

    it('delete a listing draft', () => {
        cy.LoginViaApi(realEmail, standardPassword);
        cy.visit('/listingprofile');
        cy.contains('View').click().then(() => {
            cy.contains('Draft');
            cy.contains('Edit Listing');
            cy.contains('Delete Listing');
            cy.contains('Automated testing');
            cy.contains('Location:');
            cy.contains('View Ratings');
        })
        cy.contains('Delete Listing').click();
        cy.contains('You have no listings');
    });

});