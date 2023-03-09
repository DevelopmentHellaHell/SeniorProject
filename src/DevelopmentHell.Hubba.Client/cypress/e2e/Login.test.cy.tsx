/// <reference types="cypress" />

let otpUrl: string = 'http://localhost:3000/otp';

//TODO

// describe('login-pass', () => {
//     cy.visit(loginUrl)

//     it('login-successful', () => {
//         cy.get('#email')
//             .type('login@gmail.com')
//             .should('have.value', 'login@gmail.com')
//         cy.get('#password')
//             .type('12345678')
//             .should('have.value', '12345678')
//         cy.get('#confirm-password')
//             .type('12345678')
//             .should('have.value', '12345678')
//         cy.get('.registration-card .buttons Button').click()

//         //navigate to Login Page
        
//         //#element_id
//         cy.get('#redirect-registration').click()
//         cy.url().should('eq', registrationUrl)
    
//         cy.get('#email')
//             .type('login@gmail.com')
//             .should('have.value', 'login@gmail.com');
//         cy.get('#password')
//             .type('12345678')
//             .should('have.value', '12345678')
//         cy.get('.login-card .buttons Button').click()
//         cy.url().should('eq', otpUrl)
//     })

// })
