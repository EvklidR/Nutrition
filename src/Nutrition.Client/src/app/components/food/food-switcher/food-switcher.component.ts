import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-food-switcher',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet
  ],
  templateUrl: './food-switcher.component.html',
  styleUrls: ['./food-switcher.component.css']
})
export class FoodSwitcherComponent implements OnInit {
  activeTab: 'products' | 'dishes' = 'products';

  constructor(
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        const child = this.route.firstChild;
        const path = child?.snapshot.url[0]?.path;

        if (path === 'products' || path === 'dishes') {
          this.activeTab = path;
        }
      });
  }

  switch(tab: 'products' | 'dishes'): void {
    this.activeTab = tab;
    this.router.navigate([tab], { relativeTo: this.route });
  }
}
