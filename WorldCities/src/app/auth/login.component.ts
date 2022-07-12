import { LoginResult } from './login-result';
import { AuthService } from './auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { BaseFormComponent } from '../base-form.component';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LoginRequest } from './login-request';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent extends BaseFormComponent implements OnInit {
  title?: string;
  loginResult?: LoginResult;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {
    super();
  }

  ngOnInit(): void {
    this.form = new FormGroup({
      email: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required),
    });
  }

  onSubmit() {
    var loginRequest = <LoginRequest>{};
    loginRequest.email = this.form.controls['email'].value;
    loginRequest.password = this.form.controls['password'].value;

    this.authService.login(loginRequest).subscribe({
      next: (result) => {
        this.loginResult = result;
        if (result.success && result.token) {
          localStorage.setItem(this.authService.tokenKey, result.token);
          this.router.navigate(['/']);
        }
      },
      error: (err) => {
        console.error(err);
        if (err.status == 401) {
          this.loginResult = err.error;
        }
      },
    });
  }
}
