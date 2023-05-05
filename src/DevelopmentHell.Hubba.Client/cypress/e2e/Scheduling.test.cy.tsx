/// <reference types="cypress" />
import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";

let baseUrl: string = Cypress.env("baseUrl") + "/";
let viewListingRoute: string = baseUrl + "viewlisting"
let schedulingRoute: string = baseUrl + "scheduling";
let testRoute: string = Cypress.env("serverUrl") + "tests/deletetablerecords"
let userAEmail: string = "user.a@gmail.com";
let userBEmail: string = "user.b@gmail.com"
let testPassword: string = "12345678";

context("Scheduling E2E Test Suite on existed test data", () => {
    before(()=>{
        // Clear all sessions include the backend and cache
        Cypress.session.clearAllSavedSessions();
    });

    describe("Public access to Scheduling View from a published Listing, PASS", () => {
        beforeEach(() => {
            cy.visit("/discover");
            cy.wait(3000);
            cy.get(".listing-card").eq(0).click();
            cy.url().should("eq", viewListingRoute);
            cy.contains("Check Calendar").click();
        })
        it("View rendered with Listing Title, Price, SideBar, Calendar on current date, PASS", () => {
            cy.url().should("eq", schedulingRoute);
            cy.get(".header").should("exist").and("be.visible");
            cy.contains("/hour").should("exist").and("be.visible");
            cy.get(".opentimeslots-sidebar").should("exist").and("be.visible");
            cy.get(".calendar").should("exist").and("be.visible");
            cy.get(".month").should("exist").and("be.visible");
        });

        it("Find listing availability by navigate through the month", () => {
            // Past dates are disabled in grey color, no functionality, PASS
            cy.get(".header").contains("<").click().click();
            cy.get(".month").contains("1").click();
            cy.get(".slots-card .info").should("exist").and("be.visible");
            cy.get(".halfday").should("not.exist");

            // Available dates are highlighted in light color, PASS"
            cy.get(".header").contains(">").click().click();
            cy.wait(500);
            cy.get(".month").contains("4").click();
            cy.wait(500);

            // Hour bars rendered by clicking highlighted date
            cy.get(".halfday").should("exist").and("be.visible");
        });

        it("Unauthenticated user get a message when trying to select time frame", () => {
            cy.wait(3000);
            cy.get(".header").contains(">").click()
            cy.get(".header").contains("<").click()
                .then(() => {
                    cy.get(".month").contains("4").click()
                        .then((response) => {
                            if (response) {
                                cy.contains("14:00").click();
                                cy.get(".error").should("exist").and("be.visible");
                            }
                        });
                });
        });
    });

    describe("Authenticated user can reserve a booking with chosen time frames, PASS", () => {
        beforeEach(() => {
            cy.LoginViaApi(userAEmail, testPassword);
            // Clear notification
            cy.visit(baseUrl + "notification");
            cy.pause();
            cy.contains("Clear All").click();
        })
        
        
        it.only("Select highlighted date, highlighted time, reserve the booking, PASS", async () => {
            // Navigate from Discover page
            cy.visit(baseUrl + "discover");
            cy.wait(3000);
            cy.get(".listing-card").eq(0).click();
            cy.url().should("eq", viewListingRoute);
            cy.contains("Check Calendar").click();

            // Test body
            cy.get(".header").contains(">").click();
            cy.wait(3000);
            cy.get(".day").eq(30).click();
            cy.contains("14:00").click();
            cy.contains("15:00").click();
            cy.get(".details").should("exist").and("be.visible");
            cy.contains("Reserve").should("exist").and("be.visible").click();
            cy.get(".confirmation-card").should("exist").and("be.visible");
            cy.contains("Yes").click()
                .then(() => {
                    cy.contains("Confirmation number").should("exist").and("be.visible");
                })
            cy.visit(baseUrl + "notification");
            cy.contains("Booking #").should("exist");
            cy.contains("CONFIRMED").should("exist");
            
                        // .then((response) => {
                        //     if (response) {
                        //         cy.contains("14:00").click();
                        //         cy.contains("15:00").click();
                        
                        
                        //     }
                        // });
            
        });

        it("User received a notification", () => {
            // Navigate to Notification page
            cy.visit(baseUrl + "notification");
            cy.contains("")
        })

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

        after(async () => {
            await Ajax.post(testRoute, { database: Database.Databases.LISTING_PROFILES, table: Database.Tables.BOOKINGS });
        })

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