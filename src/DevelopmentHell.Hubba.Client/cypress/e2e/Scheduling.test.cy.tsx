/// <reference types="cypress" />
import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";

let baseUrl: string = Cypress.env("baseUrl") + "/";
let viewListingRoute: string = baseUrl + "viewlisting"
let schedulingRoute: string = baseUrl + "scheduling";
let deleteDatabaseRoute: string = Cypress.env("serverUrl") + "/tests/deletedatabaserecords";
let deleteTablesRoute: string = Cypress.env("serverUrl") + "/tests/deletetablerecords";
let listingProfilesDatabase: string = Database.Databases.LISTING_PROFILES;
let notificationDatabase: string = Database.Databases.NOTIFICATIONS;
let listingHistoryTable: string = Database.Tables.LISTING_HISTORY;
let bookingsTable: string = Database.Tables.BOOKINGS;
let userAEmail: string = "user.a@gmail.com";
let hostEmail: string = "jettsonoda@gmail.com";
let testPassword: string = "12345678";

context("Scheduling E2E Test Suite on existed test data", () => {
    before(() => {
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
        })

        after(() => {
            cy.request('POST', deleteTablesRoute, { database: listingProfilesDatabase, table: listingHistoryTable });
            cy.request('POST', deleteTablesRoute, { database: listingProfilesDatabase, table: bookingsTable });
            cy.request('POST', deleteDatabaseRoute, { database: notificationDatabase });
        })

        it("Select highlighted date, highlighted time, reserve the booking, PASS", () => {
            // Navigate from Discover page
            cy.visit(baseUrl + "discover");
            cy.wait(2000);
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
            cy.contains("Booking confirmed.").should("exist").and("be.visible");


            // User received notification
            cy.visit(baseUrl + "notification");
            cy.wait(2000);
            cy.contains("Booking #").should("exist");
            cy.contains("CONFIRMED").should("exist");

            // Host received notification
            cy.get('.dropdown-content').invoke('show');
            cy.contains('Logout').click();
            cy.LoginViaApi(hostEmail, testPassword);
            cy.visit(baseUrl + "notification");
            cy.wait(2000);
            cy.contains("Booking #").should("exist");
            cy.contains("CONFIRMED").should("exist");
        });
    });

    describe.only("Owner can't book their own listing",() => {
        it("Owner can't book their own listing, message displayed, PASS", () => {
            cy.LoginViaApi(hostEmail, testPassword);
            cy.visit("/listingprofile");
            cy.contains("View").click();
            cy.wait(2000);
            cy.contains("Check Calendar").click();
            cy.get(".header").contains(">").click();
            cy.wait(3000);
            cy.get(".day").eq(30).click();
            cy.contains("9:00").click();
            cy.contains("Reserve").click();
            cy.get(".error").should("exist").and("be.visible");
        });
    })

    describe("User cancels their booing in Scheduling History, Kevin's responsilble, FAILED", () => {

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