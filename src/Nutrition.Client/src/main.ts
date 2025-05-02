import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app-routing.config';
import { marked } from 'marked';

marked.setOptions({
  breaks: true
});

bootstrapApplication(AppComponent, appConfig);
