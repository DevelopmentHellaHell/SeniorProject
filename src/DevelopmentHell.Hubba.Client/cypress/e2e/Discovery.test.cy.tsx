/// <reference types="cypress" />
/* cy.get(), cy.contains()
** https://filiphric.com/cypress-basics-selecting-elements
** https://on.cypress.io/type
*/

let discover: string = 'http://localhost:3000/discover';

describe('attempt-to-access-page', () => {
    it('logged-out', () => {
        cy.visit(discover);
        cy.get('#discover-container').contains('Search');
    });

    // it('logged-in', () => {
    //     Cypress.session.clearAllSavedSessions();
    //     cy.LoginViaApi("usera@gmail.com", "12345678");

    //     cy.visit(discover);
    //     cy.get('#discover-container').contains('Search');
    // });
});

describe('curated-results', () => {
    let realEmail: string = Cypress.env("realEmail");
    let standardPassword: string = Cypress.env("standardPassword");

    it('visit-page-and-load-curated-data', () => {
        cy.visit(discover);
        cy.get("#curated-view-wrapper").contains("Listings");
        cy.get("#curated-view-wrapper").contains("Project Showcases");
        cy.get("#curated-view-wrapper").contains("Collaborators");
    });
})

describe('search-results', () => {
    let realEmail: string = Cypress.env("realEmail");
    let standardPassword: string = Cypress.env("standardPassword");
    let testsRoute: string = '/tests/deleteDatabaseRecords';

    it('visit-page-and-search-data', () => {
        cy.visit(discover);
        cy.get("#discover-content").get("#search-input").type("woodworking");
        cy.get("#discover-content").get("#search-button").click();
        cy.get("#discover-content").contains("Results:");
    });

    it('empty-search', () => {
        cy.visit(discover);
        cy.get("#discover-content").get("#search-button").click();
        
        cy.get("#discover-content").get(".error").should("exist").and("be.visible");
    });

    it('long-search-query', () => {
        cy.visit(discover);
        var query = "";
        for (var i = 0; i < 201; i++) {
            query += "a";
        }
        cy.get("#discover-content").get("#search-input").type(query);
        cy.get("#discover-content").get("#search-button").click();

        cy.get("#discover-content").get(".error").should("exist").and("be.visible");
    });

    it('visit-page-and-search-with-catgeory-listings-with-no-results', () => {
        cy.visit(discover);
        cy.get("#discover-content").get("#search-input").type("asdasdasd");
        cy.get("#discover-content").get("#category").get("#category-dropdown").invoke("show");
        cy.get("#discover-content").get("#category").get("#category-listings").click();
        cy.get("#discover-content").get("#search-button").click();

        cy.get("#discover-content").contains("Results: 0").should("exist").and("be.visible");
        cy.get("#discover-content").get(".listing-card").should("not.exist");
    });

    it('visit-page-and-search-with-catgeory-listings-with-results', () => {
        cy.visit(discover);
        cy.get("#discover-content").get("#search-input").type("best");
        cy.get("#discover-content").get("#category").get("#category-dropdown").invoke("show");
        cy.get("#discover-content").get("#category").get("#category-listings").click();
        cy.get("#discover-content").get("#search-button").click();

        cy.get("#discover-content").contains("Results").should("exist").and("be.visible");
        cy.get("#discover-content").get(".listing-card").should("exist").and("be.visible");
    });

    it('visit-page-and-search-with-catgeory-collaborators-with-no-results', () => {
        cy.visit(discover);
        cy.get("#discover-content").get("#search-input").type("asdasdasd");
        cy.get("#discover-content").get("#category").get("#category-dropdown").invoke("show");
        cy.get("#discover-content").get("#category").get("#category-collaborators").click();
        cy.get("#discover-content").get("#search-button").click();

        cy.get("#discover-content").contains("Results: 0").should("exist").and("be.visible");
        cy.get("#discover-content").get(".collaborator-card").should("not.exist");
    });

    it('visit-page-and-search-with-catgeory-collaborators-with-results', () => {
        cy.visit(discover);
        cy.get("#discover-content").get("#search-input").type("best");
        cy.get("#discover-content").get("#category").get("#category-dropdown").invoke("show");
        cy.get("#discover-content").get("#category").get("#category-collaborators").click();
        cy.get("#discover-content").get("#search-button").click();

        cy.get("#discover-content").contains("Results").should("exist").and("be.visible");
        cy.get("#discover-content").get(".collaborator-card").should("exist").and("be.visible");
    });

    it('visit-page-and-search-with-catgeory-project-showcases-with-no-results', () => {
        cy.visit(discover);
        cy.get("#discover-content").get("#search-input").type("asdasdasd");
        cy.get("#discover-content").get("#category").get("#category-dropdown").invoke("show");
        cy.get("#discover-content").get("#category").get("#category-project-showcases").click();
        cy.get("#discover-content").get("#search-button").click();

        cy.get("#discover-content").contains("Results: 0").should("exist").and("be.visible");
        cy.get("#discover-content").get(".project-showcase-card").should("not.exist");
    });

    it('visit-page-and-search-with-catgeory-project-showcases-with-results', () => {
        cy.visit(discover);
        cy.get("#discover-content").get("#search-input").type("description1");
        cy.get("#discover-content").get("#category").get("#category-dropdown").invoke("show");
        cy.get("#discover-content").get("#category").get("#category-project-showcases").click();
        cy.get("#discover-content").get("#search-button").click();

        cy.get("#discover-content").contains("Results").should("exist").and("be.visible");
        cy.get("#discover-content").get(".project-showcase-card").should("exist").and("be.visible");
    });

    it('visit-page-and-search-with-filter-with-no-results', () => {
        cy.visit(discover);
        cy.get("#discover-content").get("#search-input").type("asdasd");
        cy.get("#discover-content").get("#filter").get("#filter-dropdown").invoke("show");
        cy.get("#discover-content").get("#filter").get("#filter-popularity").click();
        cy.get("#discover-content").get("#search-button").click();

        cy.get("#discover-content").contains("Results: 0").should("exist").and("be.visible");
        cy.get("#discover-content").get(".project-showcase-card").should("not.exist");
    });

    it('visit-page-and-search-with-filter-with-results', () => {
        cy.visit(discover);
        cy.get("#discover-content").get("#search-input").type("best");
        cy.get("#discover-content").get("#filter").get("#filter-dropdown").invoke("show");
        cy.get("#discover-content").get("#filter").get("#filter-popularity").click();
        cy.get("#discover-content").get("#search-button").click();

        cy.get("#discover-content").contains("Results").should("exist").and("be.visible");
        cy.get("#discover-content").get(".listing-card").should("exist").and("be.visible");
    });
});