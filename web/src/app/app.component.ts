import { Component } from '@angular/core';
import {RouterModule, RouterOutlet} from '@angular/router';
import * as Clients from "./api-clients";
import {environment} from "../environments/environment";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'mamkin-investor';
}
