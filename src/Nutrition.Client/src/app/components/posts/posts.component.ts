import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PostService } from '../../services/post-service/post.service';
import { PostModel } from '../../models/post-service/Responses/post.model';
import { PostsModel } from '../../models/post-service/Responses/posts.model';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faComment, faHeart as solidHeart } from '@fortawesome/free-solid-svg-icons';
import { faHeart as regularHeart } from '@fortawesome/free-regular-svg-icons';
import { Router } from '@angular/router';
import { MarkdownModule, provideMarkdown } from 'ngx-markdown';

@Component({
  selector: 'app-posts',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    FontAwesomeModule,
    MarkdownModule
  ],
  providers: [
    provideMarkdown()
  ],
  templateUrl: './posts.component.html',
  styleUrls: ['./posts.component.css']
})
export class PostsComponent implements OnInit {
  posts: PostModel[] = [];
  totalCount: number = 0;
  page: number = 1;
  pageSize: number = 10;
  searchText: string = '';
  isLoading: boolean = false;
  isMyPosts: boolean = false;
  hasNextPage: boolean = false;

  faComment = faComment;
  solidHeart = solidHeart;
  regularHeart = regularHeart;

  constructor(
    private postService: PostService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadPosts();
  }

  loadPosts(): void {
    this.isLoading = true;
    const words = this.searchText ? this.searchText.trim().split(/\s+/) : undefined;

    if (this.isMyPosts) {
      this.postService.getUserPosts(this.page, this.pageSize).subscribe({
        next: (data: PostsModel) => {
          this.posts = data.posts;
          this.totalCount = data.totalCount;
          this.isLoading = false;
          this.hasNextPage = (this.page || 1) < this.totalPages();
        },
        error: (error) => {
          console.error('Ошибка загрузки моих постов:', error);
          this.isLoading = false;
        }
      });
    } else {
      this.postService.getPosts(words, this.page, this.pageSize).subscribe({
        next: (data: PostsModel) => {
          this.posts = data.posts;
          this.totalCount = data.totalCount;
          this.isLoading = false;
          this.hasNextPage = this.page < this.totalPages();
        },
        error: (error) => {
          console.error('Ошибка загрузки постов:', error);
          this.isLoading = false;
        }
      });
    }
  }

  switchToAllPosts(): void {
    this.isMyPosts = false;
    this.page = 1;
    this.loadPosts();
  }

  switchToMyPosts(): void {
    this.isMyPosts = true;
    this.page = 1;
    this.loadPosts();
  }

  searchPosts(): void {
    this.page = 1;
    this.loadPosts();
  }

  changePage(offset: number): void {
    const newPage = this.page + offset;
    if (newPage < 1 || newPage > this.totalPages()) {
      return;
    }
    this.page = newPage;
    this.loadPosts();
  }

  totalPages(): number {
    return Math.ceil(this.totalCount / (this.pageSize || 1));
  }

  likePost(post: PostModel, event: Event): void {
    event.stopPropagation()
    this.postService.likePost(post.id).subscribe({
      next: () => {
        post.isLiked = !post.isLiked;
        post.amountOfLikes += post.isLiked ? 1 : -1;
      },
      error: (error) => {
        console.error('Ошибка лайка поста:', error);
      }
    });
  }

  navigateToCreationPost(): void {
    this.router.navigate(['/create-post']);
  }

  navigateToPost(postId: string): void {
    this.router.navigate(['/post-details', postId]);
  }
}
