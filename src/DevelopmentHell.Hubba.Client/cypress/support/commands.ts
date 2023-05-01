/// <reference types="cypress" />
export { }
declare global {
    namespace Cypress {
        interface Chainable {
            RegisterViaApi(email: string, password: string): Chainable<void>;
            LoginViaUI(email: string, password: string): Chainable<void>;
            LoginViaApi(email: string, password: string): Chainable<void>;
            LogInandOut(): Chainable<void>;
            CreateShowcase(showcaseId, title, description, files:File[]): Chainable<void>;
            CreateSampleCollaborator(): Chainable<void>;
        }
    }
};

/**
 * Register new account by direct AJAX HTTP POST to API
 * @param: email, password
*/
Cypress.Commands.add('RegisterViaApi', (email: string, password: string) => {
    cy.request('POST', Cypress.env('serverUrl') + "/registration/register", { email, password })
        .its('status').should('eq', 200);
});

/**
 * Login by direct AJAX HTTP POST to API
 * @param: email, password
 */
Cypress.Commands.add('LoginViaApi', (email: string, password: string) => {
    cy.session([email, password], () => {
        cy.request('POST', Cypress.env('serverUrl') + "/authentication/login", { email, password })
            .then(() => {
                cy.request('GET', Cypress.env('serverUrl') + "/tests/getotp")
                    .then((response) => {
                        cy.wrap(response.body).as('returnedOtp');
                        cy.get('@returnedOtp')
                            .then((otpString) => {
                                let otp = otpString.toString();
                                cy.request('POST', Cypress.env('serverUrl')+'/authentication/otp', {otp});
                            })
                    });
            })
        })
});
/**
 * Login with email and password via UI
 * Valid email, password will enable OTP input
 * Get OTP by direct AJAX HTTP GET to API via tests route
 * @param: email, password
 */
Cypress.Commands.add('LoginViaUI', (email: string, password: string) => {
    cy.session([email, password], () => {
        cy.visit("/login");
        let startTimer: number;
        cy.get('#email').as('email').type(email).should('have.value', email);
        cy.get('#password').as('password').type(password).should('have.value', password);
        cy.contains('Submit').click()
            .then(() => {
                //valid email, password, show OTP card
                cy.get('.otp-card').should('exist').and('be.visible');
            })

        //get OTP from the database
        cy.request('GET', Cypress.env('serverUrl') + "/tests/getotp")
            .then((response) => {
                cy.wrap(response.body).as('returnedOtp');
                cy.get('@returnedOtp')
                    .then((otpString) => {
                        let otp = otpString.toString();
                        cy.get('#otp').type(otp).should('have.value', otp);
                        startTimer = Date.now();
                        // time the login operation            
                        cy.contains('Submit').click()
                            .then(() => {
                                let endTimer: number = Date.now() - startTimer;
                                expect(endTimer).lessThan(5000);
                            });
                        cy.url().should('eq', Cypress.env('baseUrl') + '/');
                        cy.get('.nav-user').should('exist').and('be.visible');
                        cy.contains('Sign Up').should('not.exist');
                        cy.contains('Login').should('not.exist');
                    });
            });
    });
});
        
Cypress.Commands.add("LogInandOut", () => {
    cy.session("logout", () => {
        cy.LoginViaApi(Cypress.env('realEmail'), Cypress.env('standardPassword'))
            .then(() => {
                cy.visit("/");
                cy.get('.dropdown-content').invoke('show');
                cy.contains('Logout').click();
            });
    });
});


Cypress.Commands.add("CreateShowcase", (showcaseId, title, description, files:File[]) => {
    const fileDataList: { Item1: string; Item2: string; }[] = [];
    Promise.all(files.map(file => new Promise<void>((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => {
          // convert data URL to base64 encoded string
          const dataUrl = reader.result as string;
          const base64String = dataUrl.split(',')[1];
  
          // add the new object with file name and base64-encoded data to the fileDataList
          const fileData = { Item1: file.name, Item2: base64String };
          fileDataList.push(fileData);
          resolve();
        };
  
        reader.onerror = reject;
      })));
    cy.request('POST', Cypress.env('serverUrl') + "/showcases/new", { showcaseId: showcaseId, title: title, description: description, files: fileDataList })
        .its('status').should('eq', 200);
});


Cypress.Commands.add("CreateSampleCollaborator", () => {
    cy.visit('/account');
    cy.contains('Collaborator Profile').click();
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
})

