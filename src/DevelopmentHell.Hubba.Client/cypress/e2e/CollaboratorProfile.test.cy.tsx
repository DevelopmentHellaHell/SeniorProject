/// <reference types="cypress" />
/* cy.get(), cy.contains()
** https://filiphric.com/cypress-basics-selecting-elements
** https://on.cypress.io/type
*/

import Button from "../../src/components/Button/Button";
import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";


let homeUrl: string = 'http://localhost:3000/'
let registrationUrl: string = 'http://localhost:3000/registration';
let loginUrl: string = 'http://localhost:3000/login';
let accountUrl: string = 'http://localhost:3000/account';
let collaboratorUrl: string = 'http://localhost:3000/collaborator'
let testsRoute: string = '/tests/deleteDatabaseRecords';



//describe.only to run a single test case
describe('working-ui', () => {
    
    before(() =>{
        //clear all sessions include the backend and cache
        Cypress.session.clearAllSavedSessions();
        cy.RegisterViaApi(Cypress.env('standardEmail'), Cypress.env('standardPassword'));
    })
    let testsRoute: string = '/tests/deleteDatabaseRecords';
    beforeEach(() => {
        // using Custom Commands
        cy.LoginViaApi(Cypress.env('standardEmail'), Cypress.env('standardPassword'));
        cy.visit(accountUrl);
        cy.contains('Collaborator Profile').click();
    });

    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
        await Ajax.post(testsRoute, { database: Database.Databases.COLLABORATORS });
    });
    
    it('working-create-collaborator-button', () => {
        cy.get('#view-button').click();
        cy.get('#edit-collaborator-header')
        .contains('Create Collaborator');
    });

    it('working-edit-collaborator-button', () => {
        cy.get('#edit-button').click();
        cy.get('#edit-collaborator-header')
        .contains('Create Collaborator');
    });
    
    it('working-remove-collaborator-button', () => {
        cy.get('#remove-button').click();
        cy.get('#remove-collaborator-header')
        .contains('Collaborator Removal');
    });

    it('working-cancel-removal-collaborator-button', () => {
        cy.get('#remove-button').click();
        cy.get('#remove-collaborator-header')
        .contains('Collaborator Removal');
        cy.get('#buttons').contains("Cancel").click();
        cy.get('#collaborator-profile-header')
        .contains('Collaborator Profile');
    });

    it('working-delete-collaborator-button', () => {
        cy.get('#delete-button').click();
        cy.get('#deletion-collaborator-header')
        .contains('Collaborator Profile Deletion');
        cy.get('#buttons').contains("Cancel").click();
        cy.get('#collaborator-profile-header')
        .contains('Collaborator Profile');
    });

});

describe('working-collaborator-view-and-edit', () => {
    
    before(() =>{
        //clear all sessions include the backend and cache
        Cypress.session.clearAllSavedSessions();
    })
    let testsRoute: string = '/tests/deleteDatabaseRecords';
    beforeEach(() => {
        // using Custom Commands
        cy.RegisterViaApi(Cypress.env('standardEmail'), Cypress.env('standardPassword'));
        cy.LoginViaApi(Cypress.env('standardEmail'), Cypress.env('standardPassword'));
        cy.visit(accountUrl);
        cy.contains('Collaborator Profile').click();
    });

    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
        await Ajax.post(testsRoute, { database: Database.Databases.COLLABORATORS });
    });

    it('create-collaborator', () => {
        cy.get('#view-button').click();
        cy.get('#edit-collaborator-header')
        .contains('Create Collaborator');
        cy.get('#published').select('Yes');
        cy.get('#name').click().type('Best carpenter this side of Kansas')
        cy.get('#newTag').click().type('table')
        cy.contains('Add Tag').click()
        cy.contains('Clear All Tags').click()
        cy.get('#newTag').click().type('make a damn good table')
        cy.contains('Add Tag').click()
        cy.get('#newTag').click().type('spruce')
        cy.contains('Add Tag').click()
        cy.get('#newTag').click().type('extra hardworking')
        cy.contains('Add Tag').click()
        cy.get('#tags').contains('make a damn good table,spruce,extra hardworking')
        cy.get('#description').click().type('It was a wild week when I started learning '
            +'how to shape wood with my planning saw. My grand pappy had just lost his dog'
            +' of 23 years and it nearly broke the old man. Seeing him damn near the point'
            +' of tears lit a fire under me and I knew what I had to do. I had never '
            +'thought of woodworking as much more than a hobby but by golly I wanted '
            +'to make him right. Ever since I have never poured less than an ounce of '
            +'my hard work and dedication into everything these fingers touch, and I know'
            +' you will also see my expertise by working with me or reaching out.',{delay: 0})
        cy.get('#contactInfo').click().type('Past the broken willow and over the winding creek. Whistle real loud.')
        cy.get('input[name=profilePic]').selectFile('cypress/fixtures/cookie.png')
        cy.get('input[name=photos]').selectFile(['cypress/fixtures/Crypto Mine.png','cypress/fixtures/Shiny Mega Rayquaza.png'])
        cy.get('form').submit()
        cy.get('#success',{timeout: 10000}).should('be.visible')
        cy.contains('Collaborator Profile').click();
        cy.get('#view-button').click();
        cy.wait(200)
        cy.get('#collaborator-page-title').contains('Best carpenter this side of Kansas')
        cy.get('#up-vote').click()
        cy.wait(500)
        cy.visit(collaboratorUrl)
        cy.pause()
        
        cy.visit(accountUrl)
        cy.contains('Collaborator Profile').click();
        cy.get('#edit-button').click();
        cy.get('#edit-collaborator-header')
        .contains('Edit Collaborator');
        cy.pause()

        cy.get('form').submit()
        cy.get('#success',{timeout: 10000}).should('be.visible')
        cy.contains('Collaborator Profile').click();
        cy.get('#view-button').click();
        cy.wait(200)
        cy.get('#collaborator-page-title').contains('Best carpenter this side of Kansas')
        
        
        cy.visit(accountUrl)
        cy.contains('Collaborator Profile').click();
        cy.get('#remove-button').click();
        cy.get('#remove-collaborator-header')
        .contains('Collaborator Removal');
        cy.get('#buttons').contains("Remove").click();
        cy.get('#success',{timeout: 10000}).should('be.visible')
        cy.visit(collaboratorUrl)
        cy.get('#collaborator-page-title').should('be.undefined')
        cy.pause()

        cy.visit(accountUrl)
        cy.contains('Collaborator Profile').click();
        cy.get('#delete-button').click();
        cy.get('#deletion-collaborator-header')
        .contains('Collaborator Profile Deletion');
        cy.get('#buttons').contains("Delete").click();

        cy.visit(collaboratorUrl)
    

        
    });
});