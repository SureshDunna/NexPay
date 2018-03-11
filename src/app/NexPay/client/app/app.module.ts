import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material';

import { AppComponent } from './app.component';
import { AppService } from './app.service';
import { MaterialModule } from './material.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  entryComponents: [AppComponent],
  imports: [
    BrowserModule,
    HttpModule,
    FormsModule,
    ReactiveFormsModule,

    MaterialModule
  ],
  providers: [
    AppService,
    MatSnackBar
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
