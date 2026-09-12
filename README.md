# ATS-Friendly CV Generator
 
A web application for students/job-seekers to create CVs that are clean and ATS-Friendly through a guided multi-step form, exporting as PDF files. Everything is free, fast, and no design experience is required.

## ✨ Features
 
- **Guided Multi-Step Creation Process** : Personal Info → Education → Experience → Projects → Certificates → Skills → Result with a visual indicator of steps at each step.
- **One Draft Per User** : starting a new CV will delete user's old draft once they get a pop-up confirmation of that.
- **Real PDF Export** : uses (Puppeteer) technology for server-side rendering to create PDF files with selectable text and clickable links (emails, phone number, GitHub, LinkedIn), not just a screenshot of the page.
- **Live Form Validation** :  live strength of user's password during the registration process and live error messages with animations.
- **Secure Authentication** : used ASP.NET Core Identity, with account lockout protection and clear login error messages.
- **Responsive design** : the design is fully responsive and usable on mobile, tablet, and desktop.

## 🛠️ Tech Stack
 
| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10) |
| Auth | ASP.NET Core Identity |
| Data Access | Entity Framework Core |
| Database | SQL Server |
| PDF Generation | PuppeteerSharp (headless Chromium) |
| Front-end | Razor Views, Bootstrap 5, vanilla JS + jQuery |
| Alerts / Modals | SweetAlert2 |
| Icons / Fonts | Font Awesome, Google Fonts |

## 🚀 Live Demo 
🔗[Try the ATS CV Generator](https://your-live-link.com)

## 👤 Author
 
**Mazen Thobian**<br>
🔗[Take a look at my Portfolio](https://mazen-thobian.tech)
 
---
 
*Built as a learning project exploring ASP.NET Core, EF Core, and PDF generation.*