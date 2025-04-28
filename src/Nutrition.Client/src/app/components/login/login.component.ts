import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../../services/user-service/user.service';
import { NgIf, NgClass } from "@angular/common"
import { NgForm, NgModel, FormsModule } from "@angular/forms"

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    NgIf,
    NgClass,
    FormsModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  errorMessage: string = '';
  formSubmitted: boolean = false;

  constructor(private userService: UserService, private router: Router) { }

  get usernameInvalid(): boolean {
    return !this.username;
  }

  get passwordInvalid(): boolean {
    return !this.password;
  }

  onSubmit(): void {
    this.formSubmitted = true;

    if (this.usernameInvalid || this.passwordInvalid) {
      this.errorMessage = 'Пожалуйста, заполните все поля.';
      return;
    }

    this.userService.login({ email: this.username, password: this.password }).subscribe({
      next: () => {
        this.router.navigate(['/home']);
      },
      error: () => {
        this.errorMessage = 'Неверное имя пользователя или пароль!';
      }
    });
  }
}
