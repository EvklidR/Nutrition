import { Component } from '@angular/core';
import { Router, ActivatedRoute, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TabOption, TabSwitcherComponent } from '../../tab-switcher/tab-switcher.component';

@Component({
  selector: 'app-history-switcher',
  standalone: true,
  imports: [CommonModule, RouterOutlet, TabSwitcherComponent],
  templateUrl: './history-switcher.component.html',
  styleUrls: ['./history-switcher.component.css']
})
export class HistorySwitcherComponent {
  tabs: TabOption[] = [
    { key: 'days', label: 'История дней' },
    { key: 'meal-plans', label: 'История планов питания' }
  ];

  constructor(
    private router: Router,
    private route: ActivatedRoute
  ) { }

  switch(tab: string): void {
    this.router.navigate([tab], { relativeTo: this.route });
  }
}
