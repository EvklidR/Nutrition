import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PostService } from '../../services/post-service/post.service';
import { CommentService } from '../../services/post-service/comment.service';
import { PostModel } from '../../models/post-service/Responses/post.model';
import { CommentModel } from '../../models/post-service/Responses/comment.model';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {faComment, faHeart as solidHeart, faPaperPlane, faEdit, faTrash, faEllipsisV} from '@fortawesome/free-solid-svg-icons';
import { faHeart as regularHeart } from '@fortawesome/free-regular-svg-icons';
import { CreateCommentModel } from '../../models/post-service/Requests/create-comment.model';
import { LineBreaksPipe } from '../../pipes/line-breaks.pipe';
import { MarkdownModule, provideMarkdown } from 'ngx-markdown';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../modals/confirm-dialog-modal/confirm-dialog.component';

@Component({
  selector: 'app-post-details',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    LineBreaksPipe,
    MarkdownModule
  ],
  providers: [
    provideMarkdown()
  ],
  templateUrl: './post-details.component.html',
  styleUrls: ['./post-details.component.css']
})
export class PostDetailsComponent implements OnInit {
  postId!: string;
  post!: PostModel;
  comments: CommentModel[] = [];
  commentControl = new FormControl('');

  editingCommentId: string | null = null;
  editCommentControl = new FormControl('');

  page: number = 1;
  pageSize: number = 10;
  isLoadingComments: boolean = false;
  hasMoreComments: boolean = true;

  showPostMenu: boolean = false;

  faComment = faComment;
  solidHeart = solidHeart;
  regularHeart = regularHeart;
  faPaperPlane = faPaperPlane;
  faEdit = faEdit;
  faTrash = faTrash;
  faEllipsisV = faEllipsisV;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private postService: PostService,
    private commentService: CommentService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.postId = this.route.snapshot.paramMap.get('id')!;
    this.loadPost();
    this.loadComments();
  }

  loadPost(): void {
    this.postService.getPost(this.postId).subscribe({
      next: (data) => {
        this.post = data;
      },
      error: (err) => {
        console.error('Ошибка загрузки поста:', err);
      }
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
      ownerEmail: '',
      ownerId: '',
      text: text
    };

    this.commentService.addComment(commentData).subscribe(newComment => {
      this.comments.unshift(newComment);
      this.post.amountOfComments++;
      this.commentControl.reset();
    });
  }

  likePost(): void {
    this.postService.likePost(this.postId).subscribe(() => {
      this.post.isLiked = !this.post.isLiked;
      this.post.amountOfLikes += this.post.isLiked ? 1 : -1;
    });
  }

  likeComment(comment: CommentModel): void {
    this.commentService.likeComment(comment.id).subscribe({
      next: () => {
        comment.isLiked = !comment.isLiked;
        comment.amountOfLikes += comment.isLiked ? 1 : -1;
      },
      error: (err) => {
        console.error('Ошибка лайка комментария:', err);
      }
    });
  }

  deleteComment(commentId: string): void {
    this.commentService.deleteComment(commentId).subscribe({
      next: () => {
        this.page = 1;
        this.comments = [];
        this.hasMoreComments = true;
        this.post.amountOfComments--;
        this.loadComments();
      },
      error: (error) => {
        console.log("Ошибка при удалении комментария: ", error);
      }
    });
  }

  onKeyDown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendComment();
    }
  }

  editComment(comment: CommentModel): void {
    this.editingCommentId = comment.id;
    this.editCommentControl.setValue(comment.text);
  }

  saveComment(comment: CommentModel): void {
    const newText = this.editCommentControl.value?.trim();
    if (!newText || newText === comment.text) {
      this.cancelEdit();
      return;
    }

    this.commentService.updateComment({ id: comment.id, text: newText }).subscribe({
      next: () => {
        comment.text = newText;
        this.cancelEdit();
      },
      error: (err) => {
        console.error('Ошибка при обновлении комментария:', err);
      }
    });
  }

  cancelEdit(): void {
    this.editingCommentId = null;
    this.editCommentControl.reset();
  }

  togglePostMenu(): void {
    this.showPostMenu = !this.showPostMenu;
  }

  editPost(): void {
    this.router.navigate(['/edit-post', this.post.id]);
  }

  openDeleteConfirmation(): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Удаление поста',
        message: 'Вы уверены, что хотите удалить этот пост?'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deletePost();
      }
    });
  }

  deletePost(): void {
    this.postService.deletePost(this.post.id).subscribe({
      next: () => {
        this.router.navigate(['/posts']);
      },
      error: (err) => {
        console.error('Ошибка при удалении поста:', err);
      }
    });
  }
}
