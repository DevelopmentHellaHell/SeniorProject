/// <reference types="cypress" />

declare namespace Cypress {
    interface Chainable<Subject = any> {
        /**
         * Custom command to ... add your description here
         * @example cy.clickOnMyJourneyInCandidateCabinet()
         */
        RegisterViaApi(email: string, password: string): void;
        LoginViaApi(email: string, password: string): void;
        LoginViaUI(email: string, password: string): void;
        
    }
}