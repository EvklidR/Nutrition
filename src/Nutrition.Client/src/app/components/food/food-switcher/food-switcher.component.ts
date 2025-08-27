import { Component } from '@angular/core';
import { Router, ActivatedRoute, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TabSwitcherComponent, TabOption } from '../../tab-switcher/tab-switcher.component';

@Component({
  selector: 'app-food-switcher',
  standalone: true,
  imports: [CommonModule, RouterOutlet, TabSwitcherComponent],
  templateUrl: './food-switcher.component.html',
  styleUrls: ['./food-switcher.component.css']
})
export class FoodSwitcherComponent {
  tabs: TabOption[] = [
    { key: 'products', label: 'Продукты' },
    { key: 'dishes', label: 'Блюда' }
  ];

  constructor(
    private router: Router,
    private route: ActivatedRoute
  ) { }

  switch(tab: string): void {
    this.router.navigate([tab], { relativeTo: this.route });
  }
}
