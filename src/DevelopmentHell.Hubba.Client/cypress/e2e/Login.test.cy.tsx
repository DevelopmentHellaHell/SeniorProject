/// <reference types="cypress" />


//TODO
describe('unauthorized user can only access certain pages', () => {
    beforeEach(() => {
        cy.visit('/login');
    })
    it('only have access to Guest NavBar', () => {
        cy.get('.nav-user').should('not.exist');
    })
    it('redirect to LoginPage when visiting VerifiedUser-only page(s) by the browser bar', () => {
        cy.visit('/account')
            .then(() => {
                cy.url().should('eq', Cypress.env("baseUrl")+"/login")
            });
    })
    it('redirect to LoginPage when visiting AdminUser-only page(s) by the browser bar', () => {
        cy.visit('/analytics');
        cy.url().should('eq', Cypress.env("baseUrl")+"/");
    })
})

describe('check working links', () => {
    beforeEach(() => {
        cy.visit("/login");
    })

    it('NavBar-Sign Up button', () => {
        cy.contains("Sign Up").click();
        cy.url().should('eq', Cypress.env("baseUrl")+"/registration");
    })

    it('NavBar-Login button', () => {
        cy.contains("Login").click();
        cy.url().should('eq', Cypress.env("baseUrl")+"/login");
    })

    it('LoginCard-Registration redirect link', () => {
        cy.get('#redirect-registration').click();
        cy.url().should('eq', Cypress.env("baseUrl")+"/registration");
    })

})

describe('login successful case', () => {
    let baseUrl = Cypress.env('baseUrl')+"/";
    let loginUrl = Cypress.env('baseUrl')+"/login";
    let registrationUrl = Cypress.env("baseUrl")+"/registration";
    let realEmail = Cypress.env("realEmail");
    // let standardEmail = Cypress.env("standardEmail");
    let standardPassword = Cypress.env("standardPassword");

    beforeEach(() => {
        cy.visit(loginUrl);
    })

    it('-with valid email, password, OTP under 5s', () => {
        cy.get('#redirect-registration').click();
        cy.url().should('eq', registrationUrl);
        cy.get('#email')
            .type(realEmail)
            .should('have.value', realEmail);
        cy.get('#password')
            .type(standardPassword)
            .should('have.value', standardPassword);
        cy.get('#confirm-password')
            .type(standardPassword)
            .should('have.value', standardPassword);
        cy.contains('Submit').click()
            .then(()=>{
                cy.url().should('eq', loginUrl)
            });

        cy.get('#email').as('email').type(realEmail);
        cy.get('@email').should('have.value', realEmail);
        cy.get('#password').as('password').type(standardPassword);
        cy.get('@password').should('have.value', standardPassword);
        cy.contains('Submit').click()
            .then(() => {
                cy.get('.otp-card')
                    .should('exist').and('be.visible');

                cy.request('GET', Cypress.env('serverUrl')+"/tests/getotp")
                    .then((response) => {
                        cy.wrap(response.body).as('returnedOtp')
                    });
                
                cy.get('@returnedOtp')
                    .then((otp) => {
                        cy.get('#otp')
                            .type(otp)
                            .should('have.value', otp);
                    });
                cy.contains('Submit').click()
                    .then(()=>{
                        cy.url().should('eq', baseUrl);
                        cy.get('.nav-user').should('exist').and('be.visible')
                    });
            })
    })
})

describe('login failed cases', () => {
    let baseUrl = Cypress.env('baseUrl')+"/";
    let loginUrl = Cypress.env('baseUrl')+"/login";
    let registrationUrl = Cypress.env("baseUrl")+"/registration";
    // let realEmail = Cypress.env("realEmail");
    let standardEmail = Cypress.env("standardEmail");
    let standardPassword = Cypress.env("standardPassword");
    
    beforeEach(() => {
        cy.visit('/login');
    })

    it('empty email input', () => {
        cy.get('#password')
            .type(standardPassword);
        cy.contains('Submit').click()
            .then(() => {
                cy.get('.login-card .error').should('exist').and('be.visible');
            });
    })

    it('empty password input', () => {
        cy.get('#email')
            .type(standardEmail);
        cy.contains('Submit').click()
            .then(() => {
                cy.get('.login-card .error').should('exist').and('be.visible');
            })
    })

    it('invalid email input', () => {
        cy.get('#email')
            .type('hello.com');
        cy.get('#password')
            .type(standardPassword);
        cy.contains('Submit').click()
            .then(() => {
                cy.get('.login-card .error').should('exist').and('be.visible');
            });
    })

    it('invalid password input', () => {
        cy.get('#email')
            .type(standardEmail);
        cy.get('#password')
            .type('123456');
        cy.contains('Submit').click()
            .then(() => {
                cy.get('.login-card .error').should('exist').and('be.visible');
            });
    })

    // it.only('login pass, OTP empty', () => {
    //     cy.registerViaApi(
    //         Cypress.env('realEmail'),
    //         Cypress.env('standardPassword'))
    //     cy.loginViaApi(
    //         Cypress.env('realEmail'),
    //         Cypress.env('standardPassword'))
    //     // cy.get('#otp').should('exist').and('be.visible')
        
    // })
})
