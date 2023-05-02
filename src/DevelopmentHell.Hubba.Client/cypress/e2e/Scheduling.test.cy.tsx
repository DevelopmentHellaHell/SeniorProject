/// <reference types="cypress" />
import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";

let schedulingRoute: string = "/scheduling";
let testEmail: string = "testEmail@gmail.com";
let testPassword: string = "12345678";

context("Scheduling E2E Test Suite", () => {
    before(()=>{
        // Clear all sessions include the backend and cache
        Cypress.session.clearAllSavedSessions();
        // Regsiter UserA, Log In, Create Listing, Add Listing Availabilities, 
        cy.RegisterViaApi(testEmail, testPassword);
        cy.LoginViaApi(testEmail, testPassword);
        cy.CreateListing("Scheduling test creating a listing");
    });
    after(() => {
        // Delete test account, listing, booking
    });
    describe("Public access to Scheduling View from a published Listing, PASS", () => {

        it("View rendered with Listing Title, Price, SideBar, Calendar on current date, PASS", () => {

        });

        it("Initial loaded view, PASS", () => {

            //"Past dates are disabled in grey color, no functionality, PASS"

            //
            

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