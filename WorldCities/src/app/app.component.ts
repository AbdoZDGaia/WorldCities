import { AuthService } from './auth/auth.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  title = 'WorldCities';

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.init();
  }
}
