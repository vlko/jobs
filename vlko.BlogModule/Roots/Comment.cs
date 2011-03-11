﻿using System;
using System.Collections.Generic;

namespace vlko.BlogModule.Roots
{
    public class Comment
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the actual version.
        /// </summary>
        /// <value>The actual version.</value>
        public virtual int ActualVersion { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        public virtual int Level { get; set; }

        /// <summary>
        /// Gets or sets the parent version.
        /// </summary>
        /// <value>The parent version.</value>
        public virtual int ParentVersion { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public virtual Content Content { get; set; }

        /// <summary>
        /// Gets or sets the parent comment.
        /// </summary>
        /// <value>The parent comment.</value>
        public virtual Comment ParentComment { get; set; }

        /// <summary>
        /// Gets or sets the topmost comment in thread (can be self).
        /// </summary>
        /// <value>The topmost comment in thread (can be self).</value>
        public virtual Comment TopComment { get; set; }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        public virtual User Owner { get; set; }

        /// <summary>
        /// Gets or sets the name of the anonymous user.
        /// </summary>
        /// <value>The name of the anonymous user.</value>
        public virtual string AnonymousName { get; set; }

        /// <summary>
        /// Gets or sets the comment versions.
        /// </summary>
        /// <value>The comment versions.</value>
        public virtual IList<CommentVersion> CommentVersions { get; set; }

    }
}


