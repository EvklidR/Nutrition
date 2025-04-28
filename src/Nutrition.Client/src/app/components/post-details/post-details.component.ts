import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PostService } from '../../services/post-service/post.service';
import { CommentService } from '../../services/post-service/comment.service';
import { PostModel } from '../../models/post-service/Responses/post.model';
import { CommentModel } from '../../models/post-service/Responses/comment.model';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faComment, faHeart as solidHeart } from '@fortawesome/free-solid-svg-icons';
import { faHeart as regularHeart } from '@fortawesome/free-regular-svg-icons';
import { CreateCommentModel } from '../../models/post-service/Requests/create-comment.model';

@Component({
  selector: 'app-post-details',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FontAwesomeModule],
  templateUrl: './post-details.component.html',
  styleUrls: ['./post-details.component.css']
})
export class PostDetailsComponent implements OnInit {
  postId!: string;
  post!: PostModel;
  comments: CommentModel[] = [];
  commentControl = new FormControl('');
  page: number = 1;
  pageSize: number = 5;
  isLoadingComments: boolean = false;
  hasMoreComments: boolean = true;

  faComment = faComment;
  solidHeart = solidHeart;
  regularHeart = regularHeart;

  constructor(
    private route: ActivatedRoute,
    private postService: PostService,
    private commentService: CommentService
  ) { }

  ngOnInit(): void {
    this.postId = this.route.snapshot.paramMap.get('id')!;
    this.loadPost();
    this.loadComments();
  }

  loadPost(): void {
    this.postService.getPosts().subscribe(data => {
      this.post = data.posts.find(p => p.id === this.postId)!;
    });
  }

  loadComments(): void {
    if (this.isLoadingComments || !this.hasMoreComments) return;
    this.isLoadingComments = true;
    this.commentService.getComments(this.postId, this.page, this.pageSize).subscribe({
      next: (data) => {
        if (data.length < this.pageSize) {
          this.hasMoreComments = false;
        }
        this.comments.push(...data);
        this.page++;
        this.isLoadingComments = false;
      },
      error: () => {
        this.isLoadingComments = false;
      }
    });
  }

  sendComment(): void {
    const text = this.commentControl.value?.trim();
    if (!text) return;

    const commentData: CreateCommentModel = {
      postId: this.postId,
      ownerEmail: null,
      ownerId: null,
      text: text
    };

    this.commentService.addComment(commentData).subscribe(newComment => {
      this.comments.unshift(newComment);
      this.commentControl.reset();
    });
  }


  likePost(): void {
    this.postService.likePost(this.postId).subscribe(() => {
      this.post.isLiked = !this.post.isLiked;
      this.post.amountOfLikes += this.post.isLiked ? 1 : -1;
    });
  }
}
