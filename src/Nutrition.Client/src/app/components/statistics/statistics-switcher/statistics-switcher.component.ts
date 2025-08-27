import { Component } from '@angular/core';
import { Router, ActivatedRoute, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TabOption, TabSwitcherComponent } from '../../tab-switcher/tab-switcher.component';

@Component({
  selector: 'app-statistics-switcher',
  standalone: true,
  imports: [CommonModule, RouterOutlet, TabSwitcherComponent],
  templateUrl: './statistics-switcher.component.html',
  styleUrls: ['./statistics-switcher.component.css']
})
export class StatisticsSwitcherComponent {
  tabs: TabOption[] = [
    { key: 'body', label: 'Данные тела' },
    { key: 'food', label: 'Данные о пище' }
  ];

  constructor(
    private router: Router,
    private route: ActivatedRoute
  ) { }

  switch(tab: string): void {
    this.router.navigate([tab], { relativeTo: this.route });
  }
}
