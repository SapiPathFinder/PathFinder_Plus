# PathFinder Plus

Link for the documentation: 
https://docs.google.com/document/d/1wzd_nLb7nKdR0zMCx9caDT7a45P-su7Mzp636FREWMU/edit

## Overview
PathFinder Plus is an innovative web-based application designed to facilitate efficient route planning and points of interest (POI) discovery. Leveraging cutting-edge technology and a user-friendly interface, PathFinder Plus aims to enhance the navigation experience for users by providing optimized routes and detailed POI information between specified coordinates.


## Functional Requirements and Product Features

### API
- Route and POI Discovery (routeAndPoi): Accepts a list of coordinates as parameters. It is designed to fetch all POIs between the provided coordinates using the OpenRouteService (ORS) API's /pois endpoint, and to calculate the distances between the POIs using the /directions endpoint.

### User Interactions
- API Responses: The system is engineered to deliver precise and timely responses to user queries, facilitating an interactive and efficient user experience.

### User Interface
- Frontend: Currently, there is no frontend interface provided. The application is designed to operate as a backend service with API endpoints.

### Components
- Directions: Responsible for computing and providing directions between POIs.
POIs: Handles the retrieval of points of interest between user-specified coordinates.
TourHub (Tentative): A proposed module for aggregating and managing tours.
Model: The data model supporting the application's core functionalities.

### Authentication and Authorization
- Open Access: PathFinder Plus is accessible without the need for user authentication or authorization, offering a hassle-free experience.

### Data Processing
- Optimal Route Calculation: Employs advanced algorithms to calculate the most efficient routes.
- Categorization and Filters: Enables POI categorization and filtering for tailored user experiences.

### Reporting
- Server Analytics: Monitors and analyzes server performance and usage statistics to ensure optimal service delivery.

### User Requirements
- Accessibility: Users require a web browser, stable internet connectivity, and a display sufficiently large for map navigation.


## Non-functional Requirements and Product Properties

### Performance
- Algorithm Complexity: Utilizes sophisticated algorithms to ensure accurate and efficient route calculation.
- Server Specifications: Hosted on Azure servers to guarantee high performance and reliability.

### Usability
- User Experience: Designed with a focus on simplicity and efficiency, requiring no user interface.

### Security
- Environment Variables: Ensures the secure handling of environment variables.
- HTTPS: Employs HTTPS to secure data transmission.

### Reliability
- Hosting: Utilizes well-trusted server hosting services to ensure application availability and reliability.

### Scalability
- Resource Management: Adapts to resource availability, with scalability options directly proportional to financial investment.

### User Expectations
- Response Times: Engineered for rapid response to user requests, enhancing user satisfaction.

### Architecture
- PathFinder Plus adopts a modular architecture to facilitate efficient data processing and user interaction:
- User Interaction Flow: User -> Frontend (N/A) <-> API <-> Backend <-> ORS API
- The Backend communicates with the OpenRouteService (ORS) API and the OpenStreetMap (OSM) data source, leveraging the latter for map data.

### Documentation and Resources
- Diagrams: Work Breakdown Structure (WBS), Product Breakdown Structure (PBS), Use-Case, Class, Activity, and Sequence diagrams are available for a comprehensive understanding of the system architecture and operations.
- Code Review: Ensures code quality and adherence to best practices through regular review processes.
Documentation: Detailed documentation is provided for all aspects of the application, from setup and configuration to usage guidelines.
