// This file can be replaced during build by using the `fileReplacements` array.
// `ng build ---prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

// https://learn.microsoft.com/en-us/visualstudio/javascript/tutorial-asp-net-core-with-angular?view=vs-2022
export const environment = {
  production: false,
  apiUrl: 'localhost:7164/api/'   // Same as SWAGGER https://localhost:7164/
};

// SEE NOTE ABOVE ON BUILD  for url usage 

// angular.json has this:
//   "defaultConfiguration": "production"

// ALSO: 
//   https://stackoverflow.com/questions/66134553/angular-external-api-call-keeps-serving-on-poort-4200-after-adding-proxy-configu
//

/*
 * In development mode, to ignore zone related error stack frames such as
 * `zone.run`, `zoneDelegate.invokeTask` for easier debugging, you can
 * import the following file, but please comment it out in production mode
 * because it will have performance impact when throw error
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
