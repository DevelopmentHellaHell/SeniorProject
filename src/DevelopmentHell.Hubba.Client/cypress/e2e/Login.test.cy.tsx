/// <reference types="cypress" />
import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";

context('UI functionality', () => {
    /**
     * Unauthenticated user restriction on the web app
     * User can only navigate to LoginPage, RegistrationPage, HomePage, DiscoverPage
    */
    describe('unauthorized user can only access certain pages', () => {
        beforeEach(() => {
            cy.visit('/login');
        });
        it('only have access to Guest NavBar', () => {
            cy.get('.nav-user').should('not.exist');
        });
        it('redirect to LoginPage when visiting VerifiedUser-only page(s) by the browser bar', () => {
            cy.visit('/account')
            cy.url().should('eq', Cypress.env("baseUrl")+"/login")
        });
        it('redirect to LoginPage when visiting AdminUser-only page(s) by the browser bar', () => {
            cy.visit('/analytics');
            cy.url().should('eq', Cypress.env("baseUrl")+"/");
        });
    });
    
    /**
     * Test all links and buttons on the page
     * Logo, NavBar links, NavBar buttons, refirect link
     */
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
});

context.only('Login operation', () => {
    let baseUrl: string = Cypress.env('baseUrl') + "/";
    let loginUrl: string = Cypress.env('baseUrl') + "/login";
    let realEmail: string = Cypress.env("realEmail");
    let standardPassword: string = Cypress.env("standardPassword");
    let testsRoute: string = '/tests/deleteDatabaseRecords';
    let startTimer: number;
    before(()=>{
        cy.RegisterViaApi(Cypress.env('realEmail'), Cypress.env('standardPassword'));
    });
    beforeEach(() => {
        cy.visit('/login');
    });

    after(()=>{
        cy.request('POST', Cypress.env('serverUrl')+testsRoute,{ database: Database.Databases.USERS })
            .then((response) => {
                expect(response.status).is.eq(200);
            });
    });
    //register an account before this text suite
    describe('SETUP', () => {
        it('register new account', () => {
            //use custom command
            });
    });
    
    /**
     * Login successfully with a valid email and password
     * OTP card is enabled, user enter OTP to complete login
     * After clicking Submit button, system responses under 5s
     * Delete test data in database after testing
     */
    describe('Successful Case - login with valid email, password, OTP', () => {
        beforeEach(() => {
            cy.visit(loginUrl);
        });

        //Delete test cases from database after the test
        after(() => {
            // await Ajax.post(testsRoute, { database: Database.Databases.USERS });
            cy.request('POST', Cypress.env('serverUrl') + testsRoute, { database: Database.Databases.USERS })
                .then((response) => {
                    expect(response.status).is.eq(200);
                });
        });

        it('-with valid email, password, OTP under 5s', () => {
            cy.get('#email').as('email').type(realEmail).should('have.value', realEmail);
            cy.get('#password').as('password').type(standardPassword).should('have.value', standardPassword);
            cy.contains('Submit').click()
                .then(() => {
                    //valid email, password, show OTP card
                    cy.get('.otp-card').should('exist').and('be.visible');
                });

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
                                    let endTimer = Date.now() - startTimer;
                                    expect(endTimer).lessThan(5000);
                                });
                            cy.url().should('eq', baseUrl);
                            cy.get('.nav-user').should('exist').and('be.visible');
                            cy.contains('Sign Up').should('not.exist');
                            cy.contains('Login').should('not.exist');
                        });
                });
        });

        /**
         * After 2 failed attempts, login successfully
         */
        it('wrong credentials', () => {

        });
    });

    describe('Failure Cases', () => {
        let standardEmail = Cypress.env("standardEmail");
        let standardPassword: string = Cypress.env("standardPassword");

        

        /**
         * either email, password or OTP input syntax error
         */
        describe('syntax error', () => {
            it('empty email input', () => {
                cy.get('#password')
                    .type(standardPassword);
                cy.contains('Submit').click()
                    .then(() => {
                        cy.get('.login-card .error').should('exist').and('be.visible');
                    });
            });

            it('empty password input', () => {
                cy.get('#email')
                    .type(standardEmail);
                cy.contains('Submit').click()
                    .then(() => {
                        cy.get('.login-card .error').should('exist').and('be.visible');
                    })
            });

            it('invalid email input', () => {
                cy.get('#email')
                    .type('hello.com');
                cy.get('#password')
                    .type(standardPassword);
                cy.contains('Submit').click()
                    .then(() => {
                        cy.get('.login-card .error').should('exist').and('be.visible');
                    });
            });

            it('invalid password input', () => {
                cy.get('#email')
                    .type(standardEmail);
                cy.get('#password')
                    .type('123456');
                cy.contains('Submit').click()
                    .then(() => {
                        cy.get('.login-card .error').should('exist').and('be.visible');
                    });
            });
        });

        /**
         * either email, password is not correct
         */
        describe('wrong credentials', () => {
            
        });
            
        /**
         * Valid email, password, OTP fails
         */
        describe('login pass, OTP failed', () => {
            /**
             * OTP syntax error
             */
            it('OTP empty/invalid/wrong', () => {
                
                cy.get('#email').as('email').type(Cypress.env('realEmail')).should('have.value', Cypress.env('realEmail'));
                cy.get('#password').as('password').type(Cypress.env('standardPassword')).should('have.value', Cypress.env('standardPassword'));
                cy.contains('Submit').click()
                    .then(() => {
                        //valid email, password, show OTP card
                        cy.get('.otp-card').should('exist').and('be.visible');
                        cy.contains('Submit').click()
                            .then(()=>{
                                cy.get('.otp-card .error').should('exist').and('be.visible');
                                cy.get('#otp').clear();
                                cy.get('#otp').type('123456').should('have.value','123456');
                                cy.contains('Submit').click()
                                    .then(()=>{
                                        cy.get('.otp-card .error').should('exist').and('be.visible');
                                        cy.get('#otp').clear();
                                    });
                            });
                    });
            });

            /**
             * OTP expired
             */
            xit('OTP expired', () => {
                cy.get('#email').as('email').type(Cypress.env('realEmail')).should('have.value', Cypress.env('realEmail'));
                cy.get('#password').as('password').type(Cypress.env('standardPassword')).should('have.value', Cypress.env('standardPassword'));
                cy.contains('Submit').click()
                    .then(()=>{
                        cy.get('.otp-card').should('exist').and('be.visible');
                        cy.request('GET', Cypress.env('serverUrl')+'/tests/getotp')
                            .then((response)=>{
                                expect(response.status).is.eq(200);
                                cy.wrap(response.body).as('returnedOtp');
                                cy.get('@returnedOtp')
                                    .then((otpString)=>{
                                        let otp:string = otpString.toString();
                                        cy.get('#otp').type(otp).should('have.value', otp);
                                        //wait 3mins before submitting OTP
                                        let now: number = Date.now();
                                        cy.clock(now);
                                        cy.tick(120000)
                                            .then(() => {
                                                cy.contains('Submit').click()
                                                    .then(() => {
                                                        cy.get('.otp-card .error').should('exist').and('be.visible');
                                                    });
                                            })
                                    });
                            });
                    });
            });
        });
    });
});

// describe('login failed case - valid email, password, empty/invalid/expired OTP', () => {
//     let testsRoute: string = '/tests/deleteDatabaseRecords';

//     beforeEach(() => {
//         cy.loginViaApi(Cypress.env("realEmail"), Cypress.env("standardPassword"));
//     });
//     // after(async () => {
//     //     await Ajax.post(testsRoute, { database: Database.Databases.USERS });
//     // })
//     it('empty/invalid OTP', () => {
//         cy.get('#otp').should('exist').and('be.visible');
//         //empty OTP
//         cy.contains('Submit').click()
//             .then(()=>{
//                 cy.get('.otp-card .error').should('exist').and('be.visible');
//             });
//         //invalid OTP
//         cy.get('#otp').type('123').should('have.value', '123');
//         cy.contains('Submit').click()
//             .then(()=>{
//                 cy.get('.otp-card .error').should('exist').and('be.visible');
//             });
//         cy.get('#otp').clear();

//         //expired OTP
//         cy.wait(180000);
//         cy.request('GET', Cypress.env('serverUrl')+'tests/getotp')
//             .then((response) => {
//                 cy.wrap(response.body).as('returnedOtp');
//                 cy.get('@returnedOtp')
//                     .then((otp)=>{
//                         cy.get('#otp').type(otp).should('have.value', otp);
//                         cy.contains('Submit').click()
//                             .then(()=>{
//                                 cy.get('.otp-card .error').should('exist').and('be.visible');
//                             });
//                     })
//             });
        
//     });
// });
