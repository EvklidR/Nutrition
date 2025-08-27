import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

export interface TabOption {
  key: string;
  label: string;
}

@Component({
  selector: 'app-tab-switcher',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tab-switcher.component.html',
  styleUrls: ['./tab-switcher.component.css']
})
export class TabSwitcherComponent implements OnInit {
  @Input() tabs: TabOption[] = [];

  @Output() tabChange = new EventEmitter<string>();

  activeTab: string = '';

  constructor(
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.activeTab = this.route.firstChild?.snapshot.url[0]?.path!
  }

  onSwitch(tabKey: string): void {
    if (this.activeTab !== tabKey) {
      this.activeTab = tabKey;
      this.tabChange.emit(tabKey);
    }
  }
}
