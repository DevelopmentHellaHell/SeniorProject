/// <reference types="cypress" />
import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";

let baseUrl: string = Cypress.env("baseUrl") + "/";
let viewListingRoute: string = baseUrl + "viewlisting"
let schedulingRoute: string = baseUrl + "scheduling";
let userAEmail: string = "user.a@gmail.com";
let userBEmail: string = "user.b@gmail.com"
let testPassword: string = "12345678";

context("Scheduling E2E Test Suite", () => {
    before(()=>{
        // Clear all sessions include the backend and cache
        Cypress.session.clearAllSavedSessions();
    });

    describe("Public access to Scheduling View from a published Listing, PASS", () => {
        beforeEach(() => {
            cy.visit("/discover");
            cy.get(".title").contains("USER A listing").click();
            cy.url().should("eq", viewListingRoute);
            cy.contains("Check Calendar").click();
        })
        it("View rendered with Listing Title, Price, SideBar, Calendar on current date, PASS", () => {
            cy.url().should("eq", schedulingRoute);
            cy.contains("USER A listing").should("exist").and("be.visible");
            cy.contains("/hour").should("exist").and("be.visible");
            cy.get(".opentimeslots-sidebar").should("exist").and("be.visible");
            cy.get(".calendar").should("exist").and("be.visible");
            cy.get(".month").should("exist").and("be.visible");
        });

        it.only("Initial loaded view, PASS", () => {
            //"Past dates are disabled in grey color, no functionality, PASS"
            cy.get(".header").contains("<").click().click();
            cy.get(".month").contains("1").click();
            cy.get(".slots-card .info").should("exist").and("be.visible");
            cy.get(".halfday").should("not.exist");
            //"Available dates are highlighted in light color, PASS"
        });

        it("User clicks '<' or '>' button to request availability for the corresponded month, PASS", () => {

        });

        it("User clicks date button to choose a date, PASS", () => {

            //"Chosen date highlighted in darker color, PASS"


            //describe("Clicked available date renders open time slots, PASS"


        });
    });

    describe("Authenticated user can reserve a booking with chosen time frames, PASS", () => {
        
        it("Unauthenticated user clicks on Open Time Slots, display message that they must be logged in, PASS", () => {

        });

        it("Owner can't book their own listing, message displayed, PASS", () => {

        });

        it("Authenticated user can see Booking summary on the Sidebar, Reserve button after choosing open time slots, PASS", () => {

        });

        it ("Reserve button clicked render Confirmation Card, PASS", () => {

        });

        it ("User clicks Yes/No to confirm booking, confirmed message displayed with Booking ID, PASS", () => {

        });

        it ("Notification Page updated with Booking ID, PASS", () => {

        });

        it ("Listing Owner got notification with the same Booking ID, PASS", () => {

        });

    });

    describe("User cancels their booing in Rental History, Kevin's responsilble, FAILED", () => {

        it("Should see their booking listed in Rental History, status CONFIRMED", () => {

        });

        it("Should be able to select a booking and click cancel", () => {

        });

        it("Should get a notification about cancelled booking", () => {

        });

        it("Listing owner should get a notification about cancelled booking", () => {

        });

        it("Rental History updated with booking status set to CANCELLED", () => {

        });

    })
});