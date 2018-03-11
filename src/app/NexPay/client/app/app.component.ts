import { Component, OnInit } from '@angular/core';
import { Http, Response } from '@angular/http';
import {
  FormControl,
  FormGroupDirective,
  NgForm,
  Validators,
  FormGroup,
  FormBuilder,
  AbstractControlDirective,
  AbstractControl } from '@angular/forms';

import {MatSnackBar} from '@angular/material';

import { AppService } from './app.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  form: FormGroup;
  submited = false;

  constructor(private _appService: AppService, private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    const numberPattern = '^([0-9]*)$';
    const alphaNumbericPattern = '^[a-zA-Z 0-9]*$';
    this.form = new FormGroup({
      bsb : new FormControl('', [Validators.required, Validators.pattern(numberPattern)]),
      accNo: new FormControl('', [ Validators.required, Validators.pattern(numberPattern)]),
      accName: new FormControl('', [ Validators.required, Validators.pattern(alphaNumbericPattern)]),
      payRef: new FormControl('', [Validators.pattern(alphaNumbericPattern)]),
      amount: new FormControl('', [ Validators.required, Validators.pattern(numberPattern)])
    });
  }

  formSubmit(personForm: FormGroup) {
    if (this.form.invalid) {
      return;
    }

    this._appService.postData(this.form.value)
      .subscribe(
        success => {
          const snackBarRef = this.snackBar.open("Payment details submited successfully.", "Ok");
          snackBarRef.afterDismissed().subscribe(() => {
            this.form.controls.bsb.setValue('');
            this.form.controls.accNo.setValue('');
            this.form.controls.accName.setValue('');
            this.form.controls.payRef.setValue('');
            this.form.controls.amount.setValue('');
          });
        },
        error => {
          this.snackBar.open("Error occured while submiting your request please try again.", "Ok");
        }
      );

    this._appService.postData(this.form.value)
      .subscribe(() => {
        const snackBarRef = this.snackBar.open("Payment details submited successfully.", "Ok");
        snackBarRef.afterDismissed().subscribe(() => {
          this.form.controls.bsb.setValue('');
          this.form.controls.accNo.setValue('');
          this.form.controls.accName.setValue('');
          this.form.controls.payRef.setValue('');
          this.form.controls.amount.setValue('');
        });
      });
  }

  shouldShowErrors(control: AbstractControlDirective | AbstractControl): boolean {
    return control &&
      control.errors &&
      (control.dirty || control.touched);
  }
}
