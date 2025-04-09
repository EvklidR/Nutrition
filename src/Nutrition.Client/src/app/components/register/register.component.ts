import { Component } from '@angular/core';
import { UserService } from '../../services/user-service/user.service';
import { RegisterUserModel } from '../../models/user-service/Requests/register-user.model'
import { Router } from '@angular/router';
import { NgIf, NgClass } from '@angular/common'
import { NgForm, NgModel, FormsModule } from '@angular/forms'

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    NgIf,
    NgClass,
    FormsModule
  ],
  providers: [UserService],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  email: string = '';
  login: string = '';
  password: string = '';
  confirmPassword: string = '';
  errorMessage: string = '';

  constructor(
    private userService: UserService,
    private router: Router
  ) { }

  onSubmit(form: any): void {
    if (!form.valid) {
      return;
    }

    const { email, password, confirmPassword } = form.value;

    let registerForm: RegisterUserModel = {
      email: email,
      password: password,
      url: ''
    };


    if (password !== confirmPassword) {
      this.errorMessage = 'Пароли не совпадают.';
      return;
    }

    this.userService.register(registerForm).subscribe({
      next: () => {
        this.errorMessage = '';
        setTimeout(() => this.router.navigate(['/']), 3000);
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Ошибка при регистрации, попробуйте снова!';
      }
    });
  }
}
