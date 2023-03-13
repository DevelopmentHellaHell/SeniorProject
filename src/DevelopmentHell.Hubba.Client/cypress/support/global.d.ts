/// <reference types="cypress" />

declare namespace Cypress {
    interface Chainable<Subject = any> {
        /**
         * Custom command to ... add your description here
         * @example cy.clickOnMyJourneyInCandidateCabinet()
         */
        Register(username: string, password: string): void;
        Login(username: string, password: string): void;
        RegisterAndLogin(username: string, password: string): void;
    }
}