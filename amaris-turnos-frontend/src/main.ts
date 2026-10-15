import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/appConfig';
import { App } from './app/app';

bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));
