# MVCRegistrationSetup

Features Implemented:

1-Anonymous users are not allowed to view pages on the site. If an anonymous user vistis the site, he is redirected to the login page.

2-I have validations in place, which will ensure data integrity and also checks if a user with the email provided already exists in the database.

3-Once the user submits the registration form, a new form appears where the admin is suppose to provide his login credentials for the email that will be used to send the email verification code to the newly registered user.

NOTE: Instead of asking admin credentials, I could have hard coded the email and password, but then since I have to push the code on GitHub I took out the hard coded part and  implemented a form that asks for the admin email and password.

4-The admin enters the email and password and submits the form. If the admin's email credentials are not correct, the same form is loaded and an error message is displayed informing the admin to check his email credentials.

5-After the admin provides the right credentials, an email verification code is emailed to the user and a message is displayed on the page informing about the successful registration.

6-If the user tries to sign in without verifying the email, he/she is not allowed to login and a message is displayed informing the user to first verify the email.

7-Once the user clicks on the link that was emailed to him/her, a message is displayed on the page informing the user about successful verification and a link to login page is displayed.

8.On the login page user is logged in and redirected to the url that he/she tried to visit before logging in or he/she is redirected to the home page.

9.On the home page I am listing all the users from the database.

10-There is a Logout button located on the top if the user is logged in. Upon clicking the logout button, the user is logged out and redirected to the login page.
