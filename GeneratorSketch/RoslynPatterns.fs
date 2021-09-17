// from https://gist.github.com/jbevain/01a083c07010bc7b7cd0 with fixes for Roslyn final syntax
// * CSharpKind -> Kind
// * Removed switch and try filters as not important to me now and broken

namespace FSharp.CodeAnalysis.CSharp

open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

module RoslynPatterns =

    let (|IdentifierNameSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? IdentifierNameSyntax as t -> Some(t.Kind(), t.Identifier)
        | _ -> None

    let (|IdentifierName|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.IdentifierName ->
            let t = n :?> IdentifierNameSyntax
            Some(t.Identifier)
        | _ -> None

    let (|QualifiedNameSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? QualifiedNameSyntax as t -> Some(t.Kind(), t.Left, t.DotToken, t.Right)
        | _ -> None

    let (|QualifiedName|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.QualifiedName ->
            let t = n :?> QualifiedNameSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|GenericNameSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? GenericNameSyntax as t -> Some(t.Kind(), t.Identifier, t.TypeArgumentList)
        | _ -> None

    let (|GenericName|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.GenericName ->
            let t = n :?> GenericNameSyntax
            Some(t.Identifier, t.TypeArgumentList)
        | _ -> None

    let (|TypeArgumentListSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? TypeArgumentListSyntax as t -> Some(t.Kind(), t.LessThanToken, t.Arguments, t.GreaterThanToken)
        | _ -> None

    let (|TypeArgumentList|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.TypeArgumentList ->
            let t = n :?> TypeArgumentListSyntax
            Some(t.Arguments)
        | _ -> None

    let (|AliasQualifiedNameSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? AliasQualifiedNameSyntax as t -> Some(t.Kind(), t.Alias, t.ColonColonToken, t.Name)
        | _ -> None

    let (|AliasQualifiedName|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AliasQualifiedName ->
            let t = n :?> AliasQualifiedNameSyntax
            Some(t.Alias, t.Name)
        | _ -> None

    let (|PredefinedTypeSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? PredefinedTypeSyntax as t -> Some(t.Kind(), t.Keyword)
        | _ -> None

    let (|PredefinedType|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.PredefinedType ->
            let t = n :?> PredefinedTypeSyntax
            Some()
        | _ -> None

    let (|ArrayTypeSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ArrayTypeSyntax as t -> Some(t.Kind(), t.ElementType, t.RankSpecifiers)
        | _ -> None

    let (|ArrayType|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ArrayType ->
            let t = n :?> ArrayTypeSyntax
            Some(t.ElementType, t.RankSpecifiers)
        | _ -> None

    let (|ArrayRankSpecifierSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ArrayRankSpecifierSyntax as t -> Some(t.Kind(), t.OpenBracketToken, t.Sizes, t.CloseBracketToken)
        | _ -> None

    let (|ArrayRankSpecifier|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ArrayRankSpecifier ->
            let t = n :?> ArrayRankSpecifierSyntax
            Some(t.Sizes)
        | _ -> None

    let (|PointerTypeSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? PointerTypeSyntax as t -> Some(t.Kind(), t.ElementType, t.AsteriskToken)
        | _ -> None

    let (|PointerType|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.PointerType ->
            let t = n :?> PointerTypeSyntax
            Some(t.ElementType)
        | _ -> None

    let (|NullableTypeSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? NullableTypeSyntax as t -> Some(t.Kind(), t.ElementType, t.QuestionToken)
        | _ -> None

    let (|NullableType|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.NullableType ->
            let t = n :?> NullableTypeSyntax
            Some(t.ElementType)
        | _ -> None

    let (|OmittedTypeArgumentSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? OmittedTypeArgumentSyntax as t -> Some(t.Kind(), t.OmittedTypeArgumentToken)
        | _ -> None

    let (|OmittedTypeArgument|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.OmittedTypeArgument ->
            let t = n :?> OmittedTypeArgumentSyntax
            Some()
        | _ -> None

    let (|ParenthesizedExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ParenthesizedExpressionSyntax as t -> Some(t.Kind(), t.OpenParenToken, t.Expression, t.CloseParenToken)
        | _ -> None

    let (|ParenthesizedExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ParenthesizedExpression ->
            let t = n :?> ParenthesizedExpressionSyntax
            Some(t.Expression)
        | _ -> None

    let (|PrefixUnaryExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? PrefixUnaryExpressionSyntax as t -> Some(t.Kind(), t.OperatorToken, t.Operand)
        | _ -> None

    let (|UnaryPlusExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.UnaryPlusExpression ->
            let t = n :?> PrefixUnaryExpressionSyntax
            Some(t.Operand)
        | _ -> None

    let (|UnaryMinusExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.UnaryMinusExpression ->
            let t = n :?> PrefixUnaryExpressionSyntax
            Some(t.Operand)
        | _ -> None

    let (|BitwiseNotExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.BitwiseNotExpression ->
            let t = n :?> PrefixUnaryExpressionSyntax
            Some(t.Operand)
        | _ -> None

    let (|LogicalNotExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.LogicalNotExpression ->
            let t = n :?> PrefixUnaryExpressionSyntax
            Some(t.Operand)
        | _ -> None

    let (|PreIncrementExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.PreIncrementExpression ->
            let t = n :?> PrefixUnaryExpressionSyntax
            Some(t.Operand)
        | _ -> None

    let (|PreDecrementExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.PreDecrementExpression ->
            let t = n :?> PrefixUnaryExpressionSyntax
            Some(t.Operand)
        | _ -> None

    let (|AddressOfExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AddressOfExpression ->
            let t = n :?> PrefixUnaryExpressionSyntax
            Some(t.Operand)
        | _ -> None

    let (|PointerIndirectionExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.PointerIndirectionExpression ->
            let t = n :?> PrefixUnaryExpressionSyntax
            Some(t.Operand)
        | _ -> None

    let (|AwaitExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AwaitExpression ->
            let t = n :?> PrefixUnaryExpressionSyntax
            Some(t.Operand)
        | _ -> None

    let (|PostfixUnaryExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? PostfixUnaryExpressionSyntax as t -> Some(t.Kind(), t.Operand, t.OperatorToken)
        | _ -> None

    let (|PostIncrementExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.PostIncrementExpression ->
            let t = n :?> PostfixUnaryExpressionSyntax
            Some(t.Operand)
        | _ -> None

    let (|PostDecrementExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.PostDecrementExpression ->
            let t = n :?> PostfixUnaryExpressionSyntax
            Some(t.Operand)
        | _ -> None

    let (|MemberAccessExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? MemberAccessExpressionSyntax as t -> Some(t.Kind(), t.Expression, t.OperatorToken, t.Name)
        | _ -> None

    let (|SimpleMemberAccessExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.SimpleMemberAccessExpression ->
            let t = n :?> MemberAccessExpressionSyntax
            Some(t.Expression, t.Name)
        | _ -> None

    let (|PointerMemberAccessExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.PointerMemberAccessExpression ->
            let t = n :?> MemberAccessExpressionSyntax
            Some(t.Expression, t.Name)
        | _ -> None

    let (|BinaryExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? BinaryExpressionSyntax as t -> Some(t.Kind(), t.Left, t.OperatorToken, t.Right)
        | _ -> None

    let (|AddExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AddExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|SubtractExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.SubtractExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|MultiplyExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.MultiplyExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|DivideExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.DivideExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|ModuloExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ModuloExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|LeftShiftExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.LeftShiftExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|RightShiftExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.RightShiftExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|LogicalOrExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.LogicalOrExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|LogicalAndExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.LogicalAndExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|BitwiseOrExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.BitwiseOrExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|BitwiseAndExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.BitwiseAndExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|ExclusiveOrExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ExclusiveOrExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|EqualsExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.EqualsExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|NotEqualsExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.NotEqualsExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|LessThanExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.LessThanExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|LessThanOrEqualExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.LessThanOrEqualExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|GreaterThanExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.GreaterThanExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|GreaterThanOrEqualExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.GreaterThanOrEqualExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|IsExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.IsExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|AsExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AsExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|CoalesceExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.CoalesceExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|SimpleAssignmentExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.SimpleAssignmentExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|AddAssignmentExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AddAssignmentExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|SubtractAssignmentExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.SubtractAssignmentExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|MultiplyAssignmentExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.MultiplyAssignmentExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|DivideAssignmentExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.DivideAssignmentExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|ModuloAssignmentExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ModuloAssignmentExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|AndAssignmentExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AndAssignmentExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|ExclusiveOrAssignmentExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ExclusiveOrAssignmentExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|OrAssignmentExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.OrAssignmentExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|LeftShiftAssignmentExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.LeftShiftAssignmentExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|RightShiftAssignmentExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.RightShiftAssignmentExpression ->
            let t = n :?> BinaryExpressionSyntax
            Some(t.Left, t.Right)
        | _ -> None

    let (|ConditionalExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ConditionalExpressionSyntax as t -> Some(t.Kind(), t.Condition, t.QuestionToken, t.WhenTrue, t.ColonToken, t.WhenFalse)
        | _ -> None

    let (|ConditionalExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ConditionalExpression ->
            let t = n :?> ConditionalExpressionSyntax
            Some(t.Condition, t.WhenTrue, t.WhenFalse)
        | _ -> None

    let (|ThisExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ThisExpressionSyntax as t -> Some(t.Kind(), t.Token)
        | _ -> None

    let (|ThisExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ThisExpression ->
            let t = n :?> ThisExpressionSyntax
            Some()
        | _ -> None

    let (|BaseExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? BaseExpressionSyntax as t -> Some(t.Kind(), t.Token)
        | _ -> None

    let (|BaseExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.BaseExpression ->
            let t = n :?> BaseExpressionSyntax
            Some()
        | _ -> None

    let (|LiteralExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? LiteralExpressionSyntax as t -> Some(t.Kind(), t.Token)
        | _ -> None

    let (|ArgListExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ArgListExpression ->
            let t = n :?> LiteralExpressionSyntax
            Some()
        | _ -> None

    let (|NumericLiteralExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.NumericLiteralExpression ->
            let t = n :?> LiteralExpressionSyntax
            Some()
        | _ -> None

    let (|StringLiteralExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.StringLiteralExpression ->
            let t = n :?> LiteralExpressionSyntax
            Some()
        | _ -> None

    let (|CharacterLiteralExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.CharacterLiteralExpression ->
            let t = n :?> LiteralExpressionSyntax
            Some()
        | _ -> None

    let (|TrueLiteralExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.TrueLiteralExpression ->
            let t = n :?> LiteralExpressionSyntax
            Some()
        | _ -> None

    let (|FalseLiteralExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.FalseLiteralExpression ->
            let t = n :?> LiteralExpressionSyntax
            Some()
        | _ -> None

    let (|NullLiteralExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.NullLiteralExpression ->
            let t = n :?> LiteralExpressionSyntax
            Some()
        | _ -> None

    let (|MakeRefExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? MakeRefExpressionSyntax as t -> Some(t.Kind(), t.Keyword, t.OpenParenToken, t.Expression, t.CloseParenToken)
        | _ -> None

    let (|MakeRefExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.MakeRefExpression ->
            let t = n :?> MakeRefExpressionSyntax
            Some(t.Expression)
        | _ -> None

    let (|RefTypeExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? RefTypeExpressionSyntax as t -> Some(t.Kind(), t.Keyword, t.OpenParenToken, t.Expression, t.CloseParenToken)
        | _ -> None

    let (|RefTypeExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.RefTypeExpression ->
            let t = n :?> RefTypeExpressionSyntax
            Some(t.Expression)
        | _ -> None

    let (|RefValueExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? RefValueExpressionSyntax as t -> Some(t.Kind(), t.Keyword, t.OpenParenToken, t.Expression, t.Comma, t.Type, t.CloseParenToken)
        | _ -> None

    let (|RefValueExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.RefValueExpression ->
            let t = n :?> RefValueExpressionSyntax
            Some(t.Expression, t.Comma, t.Type)
        | _ -> None

    let (|CheckedExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? CheckedExpressionSyntax as t -> Some(t.Kind(), t.Keyword, t.OpenParenToken, t.Expression, t.CloseParenToken)
        | _ -> None

    let (|CheckedExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.CheckedExpression ->
            let t = n :?> CheckedExpressionSyntax
            Some(t.Expression)
        | _ -> None

    let (|UncheckedExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.UncheckedExpression ->
            let t = n :?> CheckedExpressionSyntax
            Some(t.Expression)
        | _ -> None

    let (|DefaultExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? DefaultExpressionSyntax as t -> Some(t.Kind(), t.Keyword, t.OpenParenToken, t.Type, t.CloseParenToken)
        | _ -> None

    let (|DefaultExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.DefaultExpression ->
            let t = n :?> DefaultExpressionSyntax
            Some(t.Type)
        | _ -> None

    let (|TypeOfExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? TypeOfExpressionSyntax as t -> Some(t.Kind(), t.Keyword, t.OpenParenToken, t.Type, t.CloseParenToken)
        | _ -> None

    let (|TypeOfExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.TypeOfExpression ->
            let t = n :?> TypeOfExpressionSyntax
            Some(t.Type)
        | _ -> None

    let (|SizeOfExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? SizeOfExpressionSyntax as t -> Some(t.Kind(), t.Keyword, t.OpenParenToken, t.Type, t.CloseParenToken)
        | _ -> None

    let (|SizeOfExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.SizeOfExpression ->
            let t = n :?> SizeOfExpressionSyntax
            Some(t.Type)
        | _ -> None

    let (|InvocationExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? InvocationExpressionSyntax as t -> Some(t.Kind(), t.Expression, t.ArgumentList)
        | _ -> None

    let (|InvocationExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.InvocationExpression ->
            let t = n :?> InvocationExpressionSyntax
            Some(t.Expression, t.ArgumentList)
        | _ -> None

    let (|ElementAccessExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ElementAccessExpressionSyntax as t -> Some(t.Kind(), t.Expression, t.ArgumentList)
        | _ -> None

    let (|ElementAccessExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ElementAccessExpression ->
            let t = n :?> ElementAccessExpressionSyntax
            Some(t.Expression, t.ArgumentList)
        | _ -> None

    let (|ArgumentListSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ArgumentListSyntax as t -> Some(t.Kind(), t.OpenParenToken, t.Arguments, t.CloseParenToken)
        | _ -> None

    let (|ArgumentList|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ArgumentList ->
            let t = n :?> ArgumentListSyntax
            Some(t.Arguments)
        | _ -> None

    let (|BracketedArgumentListSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? BracketedArgumentListSyntax as t -> Some(t.Kind(), t.OpenBracketToken, t.Arguments, t.CloseBracketToken)
        | _ -> None

    let (|BracketedArgumentList|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.BracketedArgumentList ->
            let t = n :?> BracketedArgumentListSyntax
            Some(t.Arguments)
        | _ -> None

    let (|ArgumentSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ArgumentSyntax as t -> Some(t.Kind(), t.NameColon, t.RefOrOutKeyword, t.Expression)
        | _ -> None

    let (|Argument|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.Argument ->
            let t = n :?> ArgumentSyntax
            Some(t.NameColon, t.Expression)
        | _ -> None

    let (|NameColonSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? NameColonSyntax as t -> Some(t.Kind(), t.Name, t.ColonToken)
        | _ -> None

    let (|NameColon|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.NameColon ->
            let t = n :?> NameColonSyntax
            Some(t.Name)
        | _ -> None

    let (|CastExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? CastExpressionSyntax as t -> Some(t.Kind(), t.OpenParenToken, t.Type, t.CloseParenToken, t.Expression)
        | _ -> None

    let (|CastExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.CastExpression ->
            let t = n :?> CastExpressionSyntax
            Some(t.Type, t.Expression)
        | _ -> None

    let (|AnonymousMethodExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? AnonymousMethodExpressionSyntax as t -> Some(t.Kind(), t.AsyncKeyword, t.DelegateKeyword, t.ParameterList, t.Block)
        | _ -> None

    let (|AnonymousMethodExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AnonymousMethodExpression ->
            let t = n :?> AnonymousMethodExpressionSyntax
            Some(t.ParameterList, t.Block)
        | _ -> None

    let (|SimpleLambdaExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? SimpleLambdaExpressionSyntax as t -> Some(t.Kind(), t.AsyncKeyword, t.Parameter, t.ArrowToken, t.Body)
        | _ -> None

    let (|SimpleLambdaExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.SimpleLambdaExpression ->
            let t = n :?> SimpleLambdaExpressionSyntax
            Some(t.Parameter, t.Body)
        | _ -> None

    let (|ParenthesizedLambdaExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ParenthesizedLambdaExpressionSyntax as t -> Some(t.Kind(), t.AsyncKeyword, t.ParameterList, t.ArrowToken, t.Body)
        | _ -> None

    let (|ParenthesizedLambdaExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ParenthesizedLambdaExpression ->
            let t = n :?> ParenthesizedLambdaExpressionSyntax
            Some(t.ParameterList, t.Body)
        | _ -> None

    let (|InitializerExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? InitializerExpressionSyntax as t -> Some(t.Kind(), t.OpenBraceToken, t.Expressions, t.CloseBraceToken)
        | _ -> None

    let (|ObjectInitializerExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ObjectInitializerExpression ->
            let t = n :?> InitializerExpressionSyntax
            Some(t.Expressions)
        | _ -> None

    let (|CollectionInitializerExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.CollectionInitializerExpression ->
            let t = n :?> InitializerExpressionSyntax
            Some(t.Expressions)
        | _ -> None

    let (|ArrayInitializerExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ArrayInitializerExpression ->
            let t = n :?> InitializerExpressionSyntax
            Some(t.Expressions)
        | _ -> None

    let (|ComplexElementInitializerExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ComplexElementInitializerExpression ->
            let t = n :?> InitializerExpressionSyntax
            Some(t.Expressions)
        | _ -> None

    let (|ObjectCreationExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ObjectCreationExpressionSyntax as t -> Some(t.Kind(), t.NewKeyword, t.Type, t.ArgumentList, t.Initializer)
        | _ -> None

    let (|ObjectCreationExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ObjectCreationExpression ->
            let t = n :?> ObjectCreationExpressionSyntax
            Some(t.Type, t.ArgumentList, t.Initializer)
        | _ -> None

    let (|AnonymousObjectMemberDeclaratorSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? AnonymousObjectMemberDeclaratorSyntax as t -> Some(t.Kind(), t.NameEquals, t.Expression)
        | _ -> None

    let (|AnonymousObjectMemberDeclarator|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AnonymousObjectMemberDeclarator ->
            let t = n :?> AnonymousObjectMemberDeclaratorSyntax
            Some(t.NameEquals, t.Expression)
        | _ -> None

    let (|AnonymousObjectCreationExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? AnonymousObjectCreationExpressionSyntax as t -> Some(t.Kind(), t.NewKeyword, t.OpenBraceToken, t.Initializers, t.CloseBraceToken)
        | _ -> None

    let (|AnonymousObjectCreationExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AnonymousObjectCreationExpression ->
            let t = n :?> AnonymousObjectCreationExpressionSyntax
            Some(t.Initializers)
        | _ -> None

    let (|ArrayCreationExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ArrayCreationExpressionSyntax as t -> Some(t.Kind(), t.NewKeyword, t.Type, t.Initializer)
        | _ -> None

    let (|ArrayCreationExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ArrayCreationExpression ->
            let t = n :?> ArrayCreationExpressionSyntax
            Some(t.Type, t.Initializer)
        | _ -> None

    let (|ImplicitArrayCreationExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ImplicitArrayCreationExpressionSyntax as t -> Some(t.Kind(), t.NewKeyword, t.OpenBracketToken, t.Commas, t.CloseBracketToken, t.Initializer)
        | _ -> None

    let (|ImplicitArrayCreationExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ImplicitArrayCreationExpression ->
            let t = n :?> ImplicitArrayCreationExpressionSyntax
            Some(t.Commas, t.Initializer)
        | _ -> None

    let (|StackAllocArrayCreationExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? StackAllocArrayCreationExpressionSyntax as t -> Some(t.Kind(), t.StackAllocKeyword, t.Type)
        | _ -> None

    let (|StackAllocArrayCreationExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.StackAllocArrayCreationExpression ->
            let t = n :?> StackAllocArrayCreationExpressionSyntax
            Some(t.Type)
        | _ -> None

    let (|QueryExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? QueryExpressionSyntax as t -> Some(t.Kind(), t.FromClause, t.Body)
        | _ -> None

    let (|QueryExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.QueryExpression ->
            let t = n :?> QueryExpressionSyntax
            Some(t.FromClause, t.Body)
        | _ -> None

    let (|QueryBodySyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? QueryBodySyntax as t -> Some(t.Kind(), t.Clauses, t.SelectOrGroup, t.Continuation)
        | _ -> None

    let (|QueryBody|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.QueryBody ->
            let t = n :?> QueryBodySyntax
            Some(t.Clauses, t.SelectOrGroup, t.Continuation)
        | _ -> None

    let (|FromClauseSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? FromClauseSyntax as t -> Some(t.Kind(), t.FromKeyword, t.Type, t.Identifier, t.InKeyword, t.Expression)
        | _ -> None

    let (|FromClause|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.FromClause ->
            let t = n :?> FromClauseSyntax
            Some(t.Type, t.Identifier, t.Expression)
        | _ -> None

    let (|LetClauseSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? LetClauseSyntax as t -> Some(t.Kind(), t.LetKeyword, t.Identifier, t.EqualsToken, t.Expression)
        | _ -> None

    let (|LetClause|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.LetClause ->
            let t = n :?> LetClauseSyntax
            Some(t.Identifier, t.Expression)
        | _ -> None

    let (|JoinClauseSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? JoinClauseSyntax as t -> Some(t.Kind(), t.JoinKeyword, t.Type, t.Identifier, t.InKeyword, t.InExpression, t.OnKeyword, t.LeftExpression, t.EqualsKeyword, t.RightExpression, t.Into)
        | _ -> None

    let (|JoinClause|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.JoinClause ->
            let t = n :?> JoinClauseSyntax
            Some(t.Type, t.Identifier, t.InExpression, t.LeftExpression, t.RightExpression, t.Into)
        | _ -> None

    let (|JoinIntoClauseSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? JoinIntoClauseSyntax as t -> Some(t.Kind(), t.IntoKeyword, t.Identifier)
        | _ -> None

    let (|JoinIntoClause|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.JoinIntoClause ->
            let t = n :?> JoinIntoClauseSyntax
            Some(t.Identifier)
        | _ -> None

    let (|WhereClauseSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? WhereClauseSyntax as t -> Some(t.Kind(), t.WhereKeyword, t.Condition)
        | _ -> None

    let (|WhereClause|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.WhereClause ->
            let t = n :?> WhereClauseSyntax
            Some(t.Condition)
        | _ -> None

    let (|OrderByClauseSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? OrderByClauseSyntax as t -> Some(t.Kind(), t.OrderByKeyword, t.Orderings)
        | _ -> None

    let (|OrderByClause|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.OrderByClause ->
            let t = n :?> OrderByClauseSyntax
            Some(t.Orderings)
        | _ -> None

    let (|OrderingSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? OrderingSyntax as t -> Some(t.Kind(), t.Expression, t.AscendingOrDescendingKeyword)
        | _ -> None

    let (|AscendingOrdering|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AscendingOrdering ->
            let t = n :?> OrderingSyntax
            Some(t.Expression)
        | _ -> None

    let (|DescendingOrdering|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.DescendingOrdering ->
            let t = n :?> OrderingSyntax
            Some(t.Expression)
        | _ -> None

    let (|SelectClauseSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? SelectClauseSyntax as t -> Some(t.Kind(), t.SelectKeyword, t.Expression)
        | _ -> None

    let (|SelectClause|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.SelectClause ->
            let t = n :?> SelectClauseSyntax
            Some(t.Expression)
        | _ -> None

    let (|GroupClauseSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? GroupClauseSyntax as t -> Some(t.Kind(), t.GroupKeyword, t.GroupExpression, t.ByKeyword, t.ByExpression)
        | _ -> None

    let (|GroupClause|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.GroupClause ->
            let t = n :?> GroupClauseSyntax
            Some(t.GroupExpression, t.ByExpression)
        | _ -> None

    let (|QueryContinuationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? QueryContinuationSyntax as t -> Some(t.Kind(), t.IntoKeyword, t.Identifier, t.Body)
        | _ -> None

    let (|QueryContinuation|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.QueryContinuation ->
            let t = n :?> QueryContinuationSyntax
            Some(t.Identifier, t.Body)
        | _ -> None

    let (|OmittedArraySizeExpressionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? OmittedArraySizeExpressionSyntax as t -> Some(t.Kind(), t.OmittedArraySizeExpressionToken)
        | _ -> None

    let (|OmittedArraySizeExpression|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.OmittedArraySizeExpression ->
            let t = n :?> OmittedArraySizeExpressionSyntax
            Some()
        | _ -> None

    let (|GlobalStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? GlobalStatementSyntax as t -> Some(t.Kind(), t.Statement)
        | _ -> None

    let (|GlobalStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.GlobalStatement ->
            let t = n :?> GlobalStatementSyntax
            Some(t.Statement)
        | _ -> None

    let (|BlockSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? BlockSyntax as t -> Some(t.Kind(), t.OpenBraceToken, t.Statements, t.CloseBraceToken)
        | _ -> None

    let (|Block|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.Block ->
            let t = n :?> BlockSyntax
            Some(t.Statements)
        | _ -> None

    let (|LocalDeclarationStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? LocalDeclarationStatementSyntax as t -> Some(t.Kind(), t.Modifiers, t.Declaration, t.SemicolonToken)
        | _ -> None

    let (|LocalDeclarationStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.LocalDeclarationStatement ->
            let t = n :?> LocalDeclarationStatementSyntax
            Some(t.Modifiers, t.Declaration)
        | _ -> None

    let (|VariableDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? VariableDeclarationSyntax as t -> Some(t.Kind(), t.Type, t.Variables)
        | _ -> None

    let (|VariableDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.VariableDeclaration ->
            let t = n :?> VariableDeclarationSyntax
            Some(t.Type, t.Variables)
        | _ -> None

    let (|VariableDeclaratorSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? VariableDeclaratorSyntax as t -> Some(t.Kind(), t.Identifier, t.ArgumentList, t.Initializer)
        | _ -> None

    let (|VariableDeclarator|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.VariableDeclarator ->
            let t = n :?> VariableDeclaratorSyntax
            Some(t.Identifier, t.ArgumentList, t.Initializer)
        | _ -> None

    let (|EqualsValueClauseSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? EqualsValueClauseSyntax as t -> Some(t.Kind(), t.EqualsToken, t.Value)
        | _ -> None

    let (|EqualsValueClause|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.EqualsValueClause ->
            let t = n :?> EqualsValueClauseSyntax
            Some(t.Value)
        | _ -> None

    let (|ExpressionStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ExpressionStatementSyntax as t -> Some(t.Kind(), t.Expression, t.SemicolonToken)
        | _ -> None

    let (|ExpressionStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ExpressionStatement ->
            let t = n :?> ExpressionStatementSyntax
            Some(t.Expression)
        | _ -> None

    let (|EmptyStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? EmptyStatementSyntax as t -> Some(t.Kind(), t.SemicolonToken)
        | _ -> None

    let (|EmptyStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.EmptyStatement ->
            let t = n :?> EmptyStatementSyntax
            Some()
        | _ -> None

    let (|LabeledStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? LabeledStatementSyntax as t -> Some(t.Kind(), t.Identifier, t.ColonToken, t.Statement)
        | _ -> None

    let (|LabeledStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.LabeledStatement ->
            let t = n :?> LabeledStatementSyntax
            Some(t.Identifier, t.Statement)
        | _ -> None

    let (|GotoStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? GotoStatementSyntax as t -> Some(t.Kind(), t.GotoKeyword, t.CaseOrDefaultKeyword, t.Expression, t.SemicolonToken)
        | _ -> None

    let (|GotoStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.GotoStatement ->
            let t = n :?> GotoStatementSyntax
            Some(t.Expression)
        | _ -> None

    let (|GotoCaseStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.GotoCaseStatement ->
            let t = n :?> GotoStatementSyntax
            Some(t.Expression)
        | _ -> None

    let (|GotoDefaultStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.GotoDefaultStatement ->
            let t = n :?> GotoStatementSyntax
            Some(t.Expression)
        | _ -> None

    let (|BreakStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? BreakStatementSyntax as t -> Some(t.Kind(), t.BreakKeyword, t.SemicolonToken)
        | _ -> None

    let (|BreakStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.BreakStatement ->
            let t = n :?> BreakStatementSyntax
            Some()
        | _ -> None

    let (|ContinueStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ContinueStatementSyntax as t -> Some(t.Kind(), t.ContinueKeyword, t.SemicolonToken)
        | _ -> None

    let (|ContinueStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ContinueStatement ->
            let t = n :?> ContinueStatementSyntax
            Some()
        | _ -> None

    let (|ReturnStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ReturnStatementSyntax as t -> Some(t.Kind(), t.ReturnKeyword, t.Expression, t.SemicolonToken)
        | _ -> None

    let (|ReturnStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ReturnStatement ->
            let t = n :?> ReturnStatementSyntax
            Some(t.Expression)
        | _ -> None

    let (|ThrowStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ThrowStatementSyntax as t -> Some(t.Kind(), t.ThrowKeyword, t.Expression, t.SemicolonToken)
        | _ -> None

    let (|ThrowStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ThrowStatement ->
            let t = n :?> ThrowStatementSyntax
            Some(t.Expression)
        | _ -> None

    let (|YieldStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? YieldStatementSyntax as t -> Some(t.Kind(), t.YieldKeyword, t.ReturnOrBreakKeyword, t.Expression, t.SemicolonToken)
        | _ -> None

    let (|YieldReturnStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.YieldReturnStatement ->
            let t = n :?> YieldStatementSyntax
            Some(t.Expression)
        | _ -> None

    let (|YieldBreakStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.YieldBreakStatement ->
            let t = n :?> YieldStatementSyntax
            Some(t.Expression)
        | _ -> None

    let (|WhileStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? WhileStatementSyntax as t -> Some(t.Kind(), t.WhileKeyword, t.OpenParenToken, t.Condition, t.CloseParenToken, t.Statement)
        | _ -> None

    let (|WhileStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.WhileStatement ->
            let t = n :?> WhileStatementSyntax
            Some(t.Condition, t.Statement)
        | _ -> None

    let (|DoStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? DoStatementSyntax as t -> Some(t.Kind(), t.DoKeyword, t.Statement, t.WhileKeyword, t.OpenParenToken, t.Condition, t.CloseParenToken, t.SemicolonToken)
        | _ -> None

    let (|DoStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.DoStatement ->
            let t = n :?> DoStatementSyntax
            Some(t.Statement, t.Condition)
        | _ -> None

    let (|ForStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ForStatementSyntax as t -> Some(t.Kind(), t.ForKeyword, t.OpenParenToken, t.Declaration, t.Initializers, t.FirstSemicolonToken, t.Condition, t.SecondSemicolonToken, t.Incrementors, t.CloseParenToken, t.Statement)
        | _ -> None

    let (|ForStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ForStatement ->
            let t = n :?> ForStatementSyntax
            Some(t.Declaration, t.Initializers, t.Condition, t.Incrementors, t.Statement)
        | _ -> None

    let (|ForEachStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ForEachStatementSyntax as t -> Some(t.Kind(), t.ForEachKeyword, t.OpenParenToken, t.Type, t.Identifier, t.InKeyword, t.Expression, t.CloseParenToken, t.Statement)
        | _ -> None

    let (|ForEachStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ForEachStatement ->
            let t = n :?> ForEachStatementSyntax
            Some(t.Type, t.Identifier, t.Expression, t.Statement)
        | _ -> None

    let (|UsingStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? UsingStatementSyntax as t -> Some(t.Kind(), t.UsingKeyword, t.OpenParenToken, t.Declaration, t.Expression, t.CloseParenToken, t.Statement)
        | _ -> None

    let (|UsingStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.UsingStatement ->
            let t = n :?> UsingStatementSyntax
            Some(t.Declaration, t.Expression, t.Statement)
        | _ -> None

    let (|FixedStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? FixedStatementSyntax as t -> Some(t.Kind(), t.FixedKeyword, t.OpenParenToken, t.Declaration, t.CloseParenToken, t.Statement)
        | _ -> None

    let (|FixedStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.FixedStatement ->
            let t = n :?> FixedStatementSyntax
            Some(t.Declaration, t.Statement)
        | _ -> None

    let (|CheckedStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? CheckedStatementSyntax as t -> Some(t.Kind(), t.Keyword, t.Block)
        | _ -> None

    let (|CheckedStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.CheckedStatement ->
            let t = n :?> CheckedStatementSyntax
            Some(t.Block)
        | _ -> None

    let (|UncheckedStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.UncheckedStatement ->
            let t = n :?> CheckedStatementSyntax
            Some(t.Block)
        | _ -> None

    let (|UnsafeStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? UnsafeStatementSyntax as t -> Some(t.Kind(), t.UnsafeKeyword, t.Block)
        | _ -> None

    let (|UnsafeStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.UnsafeStatement ->
            let t = n :?> UnsafeStatementSyntax
            Some(t.Block)
        | _ -> None

    let (|LockStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? LockStatementSyntax as t -> Some(t.Kind(), t.LockKeyword, t.OpenParenToken, t.Expression, t.CloseParenToken, t.Statement)
        | _ -> None

    let (|LockStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.LockStatement ->
            let t = n :?> LockStatementSyntax
            Some(t.Expression, t.Statement)
        | _ -> None

    let (|IfStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? IfStatementSyntax as t -> Some(t.Kind(), t.IfKeyword, t.OpenParenToken, t.Condition, t.CloseParenToken, t.Statement, t.Else)
        | _ -> None

    let (|IfStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.IfStatement ->
            let t = n :?> IfStatementSyntax
            Some(t.Condition, t.Statement, t.Else)
        | _ -> None

    let (|ElseClauseSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ElseClauseSyntax as t -> Some(t.Kind(), t.ElseKeyword, t.Statement)
        | _ -> None

    let (|ElseClause|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ElseClause ->
            let t = n :?> ElseClauseSyntax
            Some(t.Statement)
        | _ -> None

    let (|SwitchStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? SwitchStatementSyntax as t -> Some(t.Kind(), t.SwitchKeyword, t.OpenParenToken, t.Expression, t.CloseParenToken, t.OpenBraceToken, t.Sections, t.CloseBraceToken)
        | _ -> None

    let (|SwitchStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.SwitchStatement ->
            let t = n :?> SwitchStatementSyntax
            Some(t.Expression, t.Sections)
        | _ -> None

    let (|SwitchSectionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? SwitchSectionSyntax as t -> Some(t.Kind(), t.Labels, t.Statements)
        | _ -> None

    let (|SwitchSection|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.SwitchSection ->
            let t = n :?> SwitchSectionSyntax
            Some(t.Labels, t.Statements)
        | _ -> None

    //let (|SwitchLabelSyntax|_|) (n: CSharpSyntaxNode) =
    //    match n with
    //    | :? SwitchLabelSyntax as t -> Some(t.Kind(), t.CaseOrDefaultKeyword, t.Value, t.ColonToken)
    //    | _ -> None

    //let (|CaseSwitchLabel|_|) (n: CSharpSyntaxNode) =
    //    match n.Kind() with
    //    | SyntaxKind.CaseSwitchLabel ->
    //        let t = n :?> SwitchLabelSyntax
    //        Some(t.Value)
    //    | _ -> None

    //let (|DefaultSwitchLabel|_|) (n: CSharpSyntaxNode) =
    //    match n.Kind() with
    //    | SyntaxKind.DefaultSwitchLabel ->
    //        let t = n :?> SwitchLabelSyntax
    //        Some(t.Value)
    //    | _ -> None

    let (|TryStatementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? TryStatementSyntax as t -> Some(t.Kind(), t.TryKeyword, t.Block, t.Catches, t.Finally)
        | _ -> None

    let (|TryStatement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.TryStatement ->
            let t = n :?> TryStatementSyntax
            Some(t.Block, t.Catches, t.Finally)
        | _ -> None

    let (|CatchClauseSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? CatchClauseSyntax as t -> Some(t.Kind(), t.CatchKeyword, t.Declaration, t.Filter, t.Block)
        | _ -> None

    let (|CatchClause|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.CatchClause ->
            let t = n :?> CatchClauseSyntax
            Some(t.Declaration, t.Filter, t.Block)
        | _ -> None

    let (|CatchDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? CatchDeclarationSyntax as t -> Some(t.Kind(), t.OpenParenToken, t.Type, t.Identifier, t.CloseParenToken)
        | _ -> None

    let (|CatchDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.CatchDeclaration ->
            let t = n :?> CatchDeclarationSyntax
            Some(t.Type, t.Identifier)
        | _ -> None

    //let (|CatchFilterClauseSyntax|_|) (n: CSharpSyntaxNode) =
    //    match n with
    //    | :? CatchFilterClauseSyntax as t -> Some(t.Kind(), t.IfKeyword, t.OpenParenToken, t.FilterExpression, t.CloseParenToken)
    //    | _ -> None

    let (|CatchFilterClause|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.CatchFilterClause ->
            let t = n :?> CatchFilterClauseSyntax
            Some(t.FilterExpression)
        | _ -> None

    let (|FinallyClauseSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? FinallyClauseSyntax as t -> Some(t.Kind(), t.FinallyKeyword, t.Block)
        | _ -> None

    let (|FinallyClause|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.FinallyClause ->
            let t = n :?> FinallyClauseSyntax
            Some(t.Block)
        | _ -> None

    let (|CompilationUnitSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? CompilationUnitSyntax as t -> Some(t.Kind(), t.Externs, t.Usings, t.AttributeLists, t.Members, t.EndOfFileToken)
        | _ -> None

    let (|CompilationUnit|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.CompilationUnit ->
            let t = n :?> CompilationUnitSyntax
            Some(t.Externs, t.Usings, t.AttributeLists, t.Members)
        | _ -> None

    let (|ExternAliasDirectiveSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ExternAliasDirectiveSyntax as t -> Some(t.Kind(), t.ExternKeyword, t.AliasKeyword, t.Identifier, t.SemicolonToken)
        | _ -> None

    let (|ExternAliasDirective|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ExternAliasDirective ->
            let t = n :?> ExternAliasDirectiveSyntax
            Some(t.Identifier)
        | _ -> None

    let (|UsingDirectiveSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? UsingDirectiveSyntax as t -> Some(t.Kind(), t.UsingKeyword, t.Alias, t.Name, t.SemicolonToken)
        | _ -> None

    let (|UsingDirective|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.UsingDirective ->
            let t = n :?> UsingDirectiveSyntax
            Some(t.Alias, t.Name)
        | _ -> None

    let (|NamespaceDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? NamespaceDeclarationSyntax as t -> Some(t.Kind(), t.NamespaceKeyword, t.Name, t.OpenBraceToken, t.Externs, t.Usings, t.Members, t.CloseBraceToken, t.SemicolonToken)
        | _ -> None

    let (|NamespaceDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.NamespaceDeclaration ->
            let t = n :?> NamespaceDeclarationSyntax
            Some(t.Name, t.Externs, t.Usings, t.Members)
        | _ -> None

    let (|AttributeListSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? AttributeListSyntax as t -> Some(t.Kind(), t.OpenBracketToken, t.Target, t.Attributes, t.CloseBracketToken)
        | _ -> None

    let (|AttributeList|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AttributeList ->
            let t = n :?> AttributeListSyntax
            Some(t.Target, t.Attributes)
        | _ -> None

    let (|AttributeTargetSpecifierSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? AttributeTargetSpecifierSyntax as t -> Some(t.Kind(), t.Identifier, t.ColonToken)
        | _ -> None

    let (|AttributeTargetSpecifier|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AttributeTargetSpecifier ->
            let t = n :?> AttributeTargetSpecifierSyntax
            Some(t.Identifier)
        | _ -> None

    let (|AttributeSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? AttributeSyntax as t -> Some(t.Kind(), t.Name, t.ArgumentList)
        | _ -> None

    let (|Attribute|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.Attribute ->
            let t = n :?> AttributeSyntax
            Some(t.Name, t.ArgumentList)
        | _ -> None

    let (|AttributeArgumentListSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? AttributeArgumentListSyntax as t -> Some(t.Kind(), t.OpenParenToken, t.Arguments, t.CloseParenToken)
        | _ -> None

    let (|AttributeArgumentList|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AttributeArgumentList ->
            let t = n :?> AttributeArgumentListSyntax
            Some(t.Arguments)
        | _ -> None

    let (|AttributeArgumentSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? AttributeArgumentSyntax as t -> Some(t.Kind(), t.NameEquals, t.NameColon, t.Expression)
        | _ -> None

    let (|AttributeArgument|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AttributeArgument ->
            let t = n :?> AttributeArgumentSyntax
            Some(t.NameEquals, t.NameColon, t.Expression)
        | _ -> None

    let (|NameEqualsSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? NameEqualsSyntax as t -> Some(t.Kind(), t.Name, t.EqualsToken)
        | _ -> None

    let (|NameEquals|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.NameEquals ->
            let t = n :?> NameEqualsSyntax
            Some(t.Name)
        | _ -> None

    let (|TypeParameterListSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? TypeParameterListSyntax as t -> Some(t.Kind(), t.LessThanToken, t.Parameters, t.GreaterThanToken)
        | _ -> None

    let (|TypeParameterList|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.TypeParameterList ->
            let t = n :?> TypeParameterListSyntax
            Some(t.Parameters)
        | _ -> None

    let (|TypeParameterSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? TypeParameterSyntax as t -> Some(t.Kind(), t.AttributeLists, t.VarianceKeyword, t.Identifier)
        | _ -> None

    let (|TypeParameter|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.TypeParameter ->
            let t = n :?> TypeParameterSyntax
            Some(t.AttributeLists, t.Identifier)
        | _ -> None

    let (|ClassDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ClassDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.Keyword, t.Identifier, t.TypeParameterList, t.BaseList, t.ConstraintClauses, t.OpenBraceToken, t.Members, t.CloseBraceToken, t.SemicolonToken)
        | _ -> None

    let (|ClassDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ClassDeclaration ->
            let t = n :?> ClassDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Identifier, t.TypeParameterList, t.BaseList, t.ConstraintClauses, t.Members)
        | _ -> None

    let (|StructDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? StructDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.Keyword, t.Identifier, t.TypeParameterList, t.BaseList, t.ConstraintClauses, t.OpenBraceToken, t.Members, t.CloseBraceToken, t.SemicolonToken)
        | _ -> None

    let (|StructDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.StructDeclaration ->
            let t = n :?> StructDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Identifier, t.TypeParameterList, t.BaseList, t.ConstraintClauses, t.Members)
        | _ -> None

    let (|InterfaceDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? InterfaceDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.Keyword, t.Identifier, t.TypeParameterList, t.BaseList, t.ConstraintClauses, t.OpenBraceToken, t.Members, t.CloseBraceToken, t.SemicolonToken)
        | _ -> None

    let (|InterfaceDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.InterfaceDeclaration ->
            let t = n :?> InterfaceDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Identifier, t.TypeParameterList, t.BaseList, t.ConstraintClauses, t.Members)
        | _ -> None

    let (|EnumDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? EnumDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.EnumKeyword, t.Identifier, t.BaseList, t.OpenBraceToken, t.Members, t.CloseBraceToken, t.SemicolonToken)
        | _ -> None

    let (|EnumDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.EnumDeclaration ->
            let t = n :?> EnumDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Identifier, t.BaseList, t.Members)
        | _ -> None

    let (|DelegateDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? DelegateDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.DelegateKeyword, t.ReturnType, t.Identifier, t.TypeParameterList, t.ParameterList, t.ConstraintClauses, t.SemicolonToken)
        | _ -> None

    let (|DelegateDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.DelegateDeclaration ->
            let t = n :?> DelegateDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.ReturnType, t.Identifier, t.TypeParameterList, t.ParameterList, t.ConstraintClauses)
        | _ -> None

    let (|EnumMemberDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? EnumMemberDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Identifier, t.EqualsValue)
        | _ -> None

    let (|EnumMemberDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.EnumMemberDeclaration ->
            let t = n :?> EnumMemberDeclarationSyntax
            Some(t.AttributeLists, t.Identifier, t.EqualsValue)
        | _ -> None

    let (|BaseListSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? BaseListSyntax as t -> Some(t.Kind(), t.ColonToken, t.Types)
        | _ -> None

    let (|BaseList|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.BaseList ->
            let t = n :?> BaseListSyntax
            Some(t.Types)
        | _ -> None

    let (|TypeParameterConstraintClauseSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? TypeParameterConstraintClauseSyntax as t -> Some(t.Kind(), t.WhereKeyword, t.Name, t.ColonToken, t.Constraints)
        | _ -> None

    let (|TypeParameterConstraintClause|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.TypeParameterConstraintClause ->
            let t = n :?> TypeParameterConstraintClauseSyntax
            Some(t.Name, t.Constraints)
        | _ -> None

    let (|ConstructorConstraintSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ConstructorConstraintSyntax as t -> Some(t.Kind(), t.NewKeyword, t.OpenParenToken, t.CloseParenToken)
        | _ -> None

    let (|ConstructorConstraint|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ConstructorConstraint ->
            let t = n :?> ConstructorConstraintSyntax
            Some()
        | _ -> None

    let (|ClassOrStructConstraintSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ClassOrStructConstraintSyntax as t -> Some(t.Kind(), t.ClassOrStructKeyword)
        | _ -> None

    let (|ClassConstraint|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ClassConstraint ->
            let t = n :?> ClassOrStructConstraintSyntax
            Some()
        | _ -> None

    let (|StructConstraint|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.StructConstraint ->
            let t = n :?> ClassOrStructConstraintSyntax
            Some()
        | _ -> None

    let (|TypeConstraintSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? TypeConstraintSyntax as t -> Some(t.Kind(), t.Type)
        | _ -> None

    let (|TypeConstraint|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.TypeConstraint ->
            let t = n :?> TypeConstraintSyntax
            Some(t.Type)
        | _ -> None

    let (|FieldDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? FieldDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.Declaration, t.SemicolonToken)
        | _ -> None

    let (|FieldDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.FieldDeclaration ->
            let t = n :?> FieldDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Declaration)
        | _ -> None

    let (|EventFieldDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? EventFieldDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.EventKeyword, t.Declaration, t.SemicolonToken)
        | _ -> None

    let (|EventFieldDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.EventFieldDeclaration ->
            let t = n :?> EventFieldDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Declaration)
        | _ -> None

    let (|ExplicitInterfaceSpecifierSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ExplicitInterfaceSpecifierSyntax as t -> Some(t.Kind(), t.Name, t.DotToken)
        | _ -> None

    let (|ExplicitInterfaceSpecifier|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ExplicitInterfaceSpecifier ->
            let t = n :?> ExplicitInterfaceSpecifierSyntax
            Some(t.Name)
        | _ -> None

    let (|MethodDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? MethodDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.ReturnType, t.ExplicitInterfaceSpecifier, t.Identifier, t.TypeParameterList, t.ParameterList, t.ConstraintClauses, t.Body, t.SemicolonToken)
        | _ -> None

    let (|MethodDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.MethodDeclaration ->
            let t = n :?> MethodDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.ReturnType, t.ExplicitInterfaceSpecifier, t.Identifier, t.TypeParameterList, t.ParameterList, t.ConstraintClauses, t.Body)
        | _ -> None

    let (|OperatorDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? OperatorDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.ReturnType, t.OperatorKeyword, t.OperatorToken, t.ParameterList, t.Body, t.SemicolonToken)
        | _ -> None

    let (|OperatorDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.OperatorDeclaration ->
            let t = n :?> OperatorDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.ReturnType, t.ParameterList, t.Body)
        | _ -> None

    let (|ConversionOperatorDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ConversionOperatorDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.ImplicitOrExplicitKeyword, t.OperatorKeyword, t.Type, t.ParameterList, t.Body, t.SemicolonToken)
        | _ -> None

    let (|ConversionOperatorDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ConversionOperatorDeclaration ->
            let t = n :?> ConversionOperatorDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Type, t.ParameterList, t.Body)
        | _ -> None

    let (|ConstructorDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ConstructorDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.Identifier, t.ParameterList, t.Initializer, t.Body, t.SemicolonToken)
        | _ -> None

    let (|ConstructorDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ConstructorDeclaration ->
            let t = n :?> ConstructorDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Identifier, t.ParameterList, t.Initializer, t.Body)
        | _ -> None

    let (|ConstructorInitializerSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ConstructorInitializerSyntax as t -> Some(t.Kind(), t.ColonToken, t.ThisOrBaseKeyword, t.ArgumentList)
        | _ -> None

    let (|BaseConstructorInitializer|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.BaseConstructorInitializer ->
            let t = n :?> ConstructorInitializerSyntax
            Some(t.ArgumentList)
        | _ -> None

    let (|ThisConstructorInitializer|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ThisConstructorInitializer ->
            let t = n :?> ConstructorInitializerSyntax
            Some(t.ArgumentList)
        | _ -> None

    let (|DestructorDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? DestructorDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.TildeToken, t.Identifier, t.ParameterList, t.Body, t.SemicolonToken)
        | _ -> None

    let (|DestructorDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.DestructorDeclaration ->
            let t = n :?> DestructorDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Identifier, t.ParameterList, t.Body)
        | _ -> None

    let (|PropertyDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? PropertyDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.Type, t.ExplicitInterfaceSpecifier, t.Identifier, t.AccessorList)
        | _ -> None

    let (|PropertyDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.PropertyDeclaration ->
            let t = n :?> PropertyDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Type, t.ExplicitInterfaceSpecifier, t.Identifier, t.AccessorList)
        | _ -> None

    let (|EventDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? EventDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.EventKeyword, t.Type, t.ExplicitInterfaceSpecifier, t.Identifier, t.AccessorList)
        | _ -> None

    let (|EventDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.EventDeclaration ->
            let t = n :?> EventDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Type, t.ExplicitInterfaceSpecifier, t.Identifier, t.AccessorList)
        | _ -> None

    let (|IndexerDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? IndexerDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.Type, t.ExplicitInterfaceSpecifier, t.ThisKeyword, t.ParameterList, t.AccessorList)
        | _ -> None

    let (|IndexerDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.IndexerDeclaration ->
            let t = n :?> IndexerDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Type, t.ExplicitInterfaceSpecifier, t.ParameterList, t.AccessorList)
        | _ -> None

    let (|AccessorListSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? AccessorListSyntax as t -> Some(t.Kind(), t.OpenBraceToken, t.Accessors, t.CloseBraceToken)
        | _ -> None

    let (|AccessorList|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AccessorList ->
            let t = n :?> AccessorListSyntax
            Some(t.Accessors)
        | _ -> None

    let (|AccessorDeclarationSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? AccessorDeclarationSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.Keyword, t.Body, t.SemicolonToken)
        | _ -> None

    let (|GetAccessorDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.GetAccessorDeclaration ->
            let t = n :?> AccessorDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Body)
        | _ -> None

    let (|SetAccessorDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.SetAccessorDeclaration ->
            let t = n :?> AccessorDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Body)
        | _ -> None

    let (|AddAccessorDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.AddAccessorDeclaration ->
            let t = n :?> AccessorDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Body)
        | _ -> None

    let (|RemoveAccessorDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.RemoveAccessorDeclaration ->
            let t = n :?> AccessorDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Body)
        | _ -> None

    let (|UnknownAccessorDeclaration|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.UnknownAccessorDeclaration ->
            let t = n :?> AccessorDeclarationSyntax
            Some(t.AttributeLists, t.Modifiers, t.Body)
        | _ -> None

    let (|ParameterListSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ParameterListSyntax as t -> Some(t.Kind(), t.OpenParenToken, t.Parameters, t.CloseParenToken)
        | _ -> None

    let (|ParameterList|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ParameterList ->
            let t = n :?> ParameterListSyntax
            Some(t.Parameters)
        | _ -> None

    let (|BracketedParameterListSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? BracketedParameterListSyntax as t -> Some(t.Kind(), t.OpenBracketToken, t.Parameters, t.CloseBracketToken)
        | _ -> None

    let (|BracketedParameterList|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.BracketedParameterList ->
            let t = n :?> BracketedParameterListSyntax
            Some(t.Parameters)
        | _ -> None

    let (|ParameterSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ParameterSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.Type, t.Identifier, t.Default)
        | _ -> None

    let (|Parameter|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.Parameter ->
            let t = n :?> ParameterSyntax
            Some(t.AttributeLists, t.Modifiers, t.Type, t.Identifier, t.Default)
        | _ -> None

    let (|IncompleteMemberSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? IncompleteMemberSyntax as t -> Some(t.Kind(), t.AttributeLists, t.Modifiers, t.Type)
        | _ -> None

    let (|IncompleteMember|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.IncompleteMember ->
            let t = n :?> IncompleteMemberSyntax
            Some(t.AttributeLists, t.Modifiers, t.Type)
        | _ -> None

    let (|SkippedTokensTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? SkippedTokensTriviaSyntax as t -> Some(t.Kind(), t.Tokens)
        | _ -> None

    let (|SkippedTokensTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.SkippedTokensTrivia ->
            let t = n :?> SkippedTokensTriviaSyntax
            Some(t.Tokens)
        | _ -> None

    let (|DocumentationCommentTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? DocumentationCommentTriviaSyntax as t -> Some(t.Kind(), t.Content, t.EndOfComment)
        | _ -> None

    let (|SingleLineDocumentationCommentTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.SingleLineDocumentationCommentTrivia ->
            let t = n :?> DocumentationCommentTriviaSyntax
            Some(t.Content, t.EndOfComment)
        | _ -> None

    let (|MultiLineDocumentationCommentTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.MultiLineDocumentationCommentTrivia ->
            let t = n :?> DocumentationCommentTriviaSyntax
            Some(t.Content, t.EndOfComment)
        | _ -> None

    let (|TypeCrefSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? TypeCrefSyntax as t -> Some(t.Kind(), t.Type)
        | _ -> None

    let (|TypeCref|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.TypeCref ->
            let t = n :?> TypeCrefSyntax
            Some(t.Type)
        | _ -> None

    let (|QualifiedCrefSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? QualifiedCrefSyntax as t -> Some(t.Kind(), t.Container, t.DotToken, t.Member)
        | _ -> None

    let (|QualifiedCref|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.QualifiedCref ->
            let t = n :?> QualifiedCrefSyntax
            Some(t.Container, t.Member)
        | _ -> None

    let (|NameMemberCrefSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? NameMemberCrefSyntax as t -> Some(t.Kind(), t.Name, t.Parameters)
        | _ -> None

    let (|NameMemberCref|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.NameMemberCref ->
            let t = n :?> NameMemberCrefSyntax
            Some(t.Name, t.Parameters)
        | _ -> None

    let (|IndexerMemberCrefSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? IndexerMemberCrefSyntax as t -> Some(t.Kind(), t.ThisKeyword, t.Parameters)
        | _ -> None

    let (|IndexerMemberCref|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.IndexerMemberCref ->
            let t = n :?> IndexerMemberCrefSyntax
            Some(t.Parameters)
        | _ -> None

    let (|OperatorMemberCrefSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? OperatorMemberCrefSyntax as t -> Some(t.Kind(), t.OperatorKeyword, t.OperatorToken, t.Parameters)
        | _ -> None

    let (|OperatorMemberCref|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.OperatorMemberCref ->
            let t = n :?> OperatorMemberCrefSyntax
            Some(t.Parameters)
        | _ -> None

    let (|ConversionOperatorMemberCrefSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ConversionOperatorMemberCrefSyntax as t -> Some(t.Kind(), t.ImplicitOrExplicitKeyword, t.OperatorKeyword, t.Type, t.Parameters)
        | _ -> None

    let (|ConversionOperatorMemberCref|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ConversionOperatorMemberCref ->
            let t = n :?> ConversionOperatorMemberCrefSyntax
            Some(t.Type, t.Parameters)
        | _ -> None

    let (|CrefParameterListSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? CrefParameterListSyntax as t -> Some(t.Kind(), t.OpenParenToken, t.Parameters, t.CloseParenToken)
        | _ -> None

    let (|CrefParameterList|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.CrefParameterList ->
            let t = n :?> CrefParameterListSyntax
            Some(t.Parameters)
        | _ -> None

    let (|CrefBracketedParameterListSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? CrefBracketedParameterListSyntax as t -> Some(t.Kind(), t.OpenBracketToken, t.Parameters, t.CloseBracketToken)
        | _ -> None

    let (|CrefBracketedParameterList|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.CrefBracketedParameterList ->
            let t = n :?> CrefBracketedParameterListSyntax
            Some(t.Parameters)
        | _ -> None

    let (|CrefParameterSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? CrefParameterSyntax as t -> Some(t.Kind(), t.RefOrOutKeyword, t.Type)
        | _ -> None

    let (|CrefParameter|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.CrefParameter ->
            let t = n :?> CrefParameterSyntax
            Some(t.Type)
        | _ -> None

    let (|XmlElementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? XmlElementSyntax as t -> Some(t.Kind(), t.StartTag, t.Content, t.EndTag)
        | _ -> None

    let (|XmlElement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.XmlElement ->
            let t = n :?> XmlElementSyntax
            Some(t.StartTag, t.Content, t.EndTag)
        | _ -> None

    let (|XmlElementStartTagSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? XmlElementStartTagSyntax as t -> Some(t.Kind(), t.LessThanToken, t.Name, t.Attributes, t.GreaterThanToken)
        | _ -> None

    let (|XmlElementStartTag|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.XmlElementStartTag ->
            let t = n :?> XmlElementStartTagSyntax
            Some(t.Name, t.Attributes)
        | _ -> None

    let (|XmlElementEndTagSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? XmlElementEndTagSyntax as t -> Some(t.Kind(), t.LessThanSlashToken, t.Name, t.GreaterThanToken)
        | _ -> None

    let (|XmlElementEndTag|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.XmlElementEndTag ->
            let t = n :?> XmlElementEndTagSyntax
            Some(t.Name)
        | _ -> None

    let (|XmlEmptyElementSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? XmlEmptyElementSyntax as t -> Some(t.Kind(), t.LessThanToken, t.Name, t.Attributes, t.SlashGreaterThanToken)
        | _ -> None

    let (|XmlEmptyElement|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.XmlEmptyElement ->
            let t = n :?> XmlEmptyElementSyntax
            Some(t.Name, t.Attributes)
        | _ -> None

    let (|XmlNameSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? XmlNameSyntax as t -> Some(t.Kind(), t.Prefix, t.LocalName)
        | _ -> None

    let (|XmlName|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.XmlName ->
            let t = n :?> XmlNameSyntax
            Some(t.Prefix, t.LocalName)
        | _ -> None

    let (|XmlPrefixSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? XmlPrefixSyntax as t -> Some(t.Kind(), t.Prefix, t.ColonToken)
        | _ -> None

    let (|XmlPrefix|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.XmlPrefix ->
            let t = n :?> XmlPrefixSyntax
            Some(t.Prefix)
        | _ -> None

    let (|XmlTextAttributeSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? XmlTextAttributeSyntax as t -> Some(t.Kind(), t.Name, t.EqualsToken, t.StartQuoteToken, t.TextTokens, t.EndQuoteToken)
        | _ -> None

    let (|XmlTextAttribute|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.XmlTextAttribute ->
            let t = n :?> XmlTextAttributeSyntax
            Some(t.Name, t.TextTokens)
        | _ -> None

    let (|XmlCrefAttributeSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? XmlCrefAttributeSyntax as t -> Some(t.Kind(), t.Name, t.EqualsToken, t.StartQuoteToken, t.Cref, t.EndQuoteToken)
        | _ -> None

    let (|XmlCrefAttribute|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.XmlCrefAttribute ->
            let t = n :?> XmlCrefAttributeSyntax
            Some(t.Name, t.Cref)
        | _ -> None

    let (|XmlNameAttributeSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? XmlNameAttributeSyntax as t -> Some(t.Kind(), t.Name, t.EqualsToken, t.StartQuoteToken, t.Identifier, t.EndQuoteToken)
        | _ -> None

    let (|XmlNameAttribute|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.XmlNameAttribute ->
            let t = n :?> XmlNameAttributeSyntax
            Some(t.Name, t.Identifier)
        | _ -> None

    let (|XmlTextSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? XmlTextSyntax as t -> Some(t.Kind(), t.TextTokens)
        | _ -> None

    let (|XmlText|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.XmlText ->
            let t = n :?> XmlTextSyntax
            Some(t.TextTokens)
        | _ -> None

    let (|XmlCDataSectionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? XmlCDataSectionSyntax as t -> Some(t.Kind(), t.StartCDataToken, t.TextTokens, t.EndCDataToken)
        | _ -> None

    let (|XmlCDataSection|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.XmlCDataSection ->
            let t = n :?> XmlCDataSectionSyntax
            Some(t.TextTokens)
        | _ -> None

    let (|XmlProcessingInstructionSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? XmlProcessingInstructionSyntax as t -> Some(t.Kind(), t.StartProcessingInstructionToken, t.Name, t.TextTokens, t.EndProcessingInstructionToken)
        | _ -> None

    let (|XmlProcessingInstruction|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.XmlProcessingInstruction ->
            let t = n :?> XmlProcessingInstructionSyntax
            Some(t.Name, t.TextTokens)
        | _ -> None

    let (|XmlCommentSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? XmlCommentSyntax as t -> Some(t.Kind(), t.LessThanExclamationMinusMinusToken, t.TextTokens, t.MinusMinusGreaterThanToken)
        | _ -> None

    let (|XmlComment|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.XmlComment ->
            let t = n :?> XmlCommentSyntax
            Some(t.TextTokens)
        | _ -> None

    let (|IfDirectiveTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? IfDirectiveTriviaSyntax as t -> Some(t.Kind(), t.HashToken, t.IfKeyword, t.Condition, t.EndOfDirectiveToken, t.IsActive, t.BranchTaken, t.ConditionValue)
        | _ -> None

    let (|IfDirectiveTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.IfDirectiveTrivia ->
            let t = n :?> IfDirectiveTriviaSyntax
            Some(t.Condition, t.IsActive, t.BranchTaken, t.ConditionValue)
        | _ -> None

    let (|ElifDirectiveTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ElifDirectiveTriviaSyntax as t -> Some(t.Kind(), t.HashToken, t.ElifKeyword, t.Condition, t.EndOfDirectiveToken, t.IsActive, t.BranchTaken, t.ConditionValue)
        | _ -> None

    let (|ElifDirectiveTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ElifDirectiveTrivia ->
            let t = n :?> ElifDirectiveTriviaSyntax
            Some(t.Condition, t.IsActive, t.BranchTaken, t.ConditionValue)
        | _ -> None

    let (|ElseDirectiveTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ElseDirectiveTriviaSyntax as t -> Some(t.Kind(), t.HashToken, t.ElseKeyword, t.EndOfDirectiveToken, t.IsActive, t.BranchTaken)
        | _ -> None

    let (|ElseDirectiveTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ElseDirectiveTrivia ->
            let t = n :?> ElseDirectiveTriviaSyntax
            Some(t.IsActive, t.BranchTaken)
        | _ -> None

    let (|EndIfDirectiveTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? EndIfDirectiveTriviaSyntax as t -> Some(t.Kind(), t.HashToken, t.EndIfKeyword, t.EndOfDirectiveToken, t.IsActive)
        | _ -> None

    let (|EndIfDirectiveTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.EndIfDirectiveTrivia ->
            let t = n :?> EndIfDirectiveTriviaSyntax
            Some(t.IsActive)
        | _ -> None

    let (|RegionDirectiveTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? RegionDirectiveTriviaSyntax as t -> Some(t.Kind(), t.HashToken, t.RegionKeyword, t.EndOfDirectiveToken, t.IsActive)
        | _ -> None

    let (|RegionDirectiveTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.RegionDirectiveTrivia ->
            let t = n :?> RegionDirectiveTriviaSyntax
            Some(t.IsActive)
        | _ -> None

    let (|EndRegionDirectiveTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? EndRegionDirectiveTriviaSyntax as t -> Some(t.Kind(), t.HashToken, t.EndRegionKeyword, t.EndOfDirectiveToken, t.IsActive)
        | _ -> None

    let (|EndRegionDirectiveTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.EndRegionDirectiveTrivia ->
            let t = n :?> EndRegionDirectiveTriviaSyntax
            Some(t.IsActive)
        | _ -> None

    let (|ErrorDirectiveTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ErrorDirectiveTriviaSyntax as t -> Some(t.Kind(), t.HashToken, t.ErrorKeyword, t.EndOfDirectiveToken, t.IsActive)
        | _ -> None

    let (|ErrorDirectiveTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ErrorDirectiveTrivia ->
            let t = n :?> ErrorDirectiveTriviaSyntax
            Some(t.IsActive)
        | _ -> None

    let (|WarningDirectiveTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? WarningDirectiveTriviaSyntax as t -> Some(t.Kind(), t.HashToken, t.WarningKeyword, t.EndOfDirectiveToken, t.IsActive)
        | _ -> None

    let (|WarningDirectiveTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.WarningDirectiveTrivia ->
            let t = n :?> WarningDirectiveTriviaSyntax
            Some(t.IsActive)
        | _ -> None

    let (|BadDirectiveTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? BadDirectiveTriviaSyntax as t -> Some(t.Kind(), t.HashToken, t.Identifier, t.EndOfDirectiveToken, t.IsActive)
        | _ -> None

    let (|BadDirectiveTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.BadDirectiveTrivia ->
            let t = n :?> BadDirectiveTriviaSyntax
            Some(t.Identifier, t.IsActive)
        | _ -> None

    let (|DefineDirectiveTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? DefineDirectiveTriviaSyntax as t -> Some(t.Kind(), t.HashToken, t.DefineKeyword, t.Name, t.EndOfDirectiveToken, t.IsActive)
        | _ -> None

    let (|DefineDirectiveTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.DefineDirectiveTrivia ->
            let t = n :?> DefineDirectiveTriviaSyntax
            Some(t.Name, t.IsActive)
        | _ -> None

    let (|UndefDirectiveTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? UndefDirectiveTriviaSyntax as t -> Some(t.Kind(), t.HashToken, t.UndefKeyword, t.Name, t.EndOfDirectiveToken, t.IsActive)
        | _ -> None

    let (|UndefDirectiveTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.UndefDirectiveTrivia ->
            let t = n :?> UndefDirectiveTriviaSyntax
            Some(t.Name, t.IsActive)
        | _ -> None

    let (|LineDirectiveTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? LineDirectiveTriviaSyntax as t -> Some(t.Kind(), t.HashToken, t.LineKeyword, t.Line, t.File, t.EndOfDirectiveToken, t.IsActive)
        | _ -> None

    let (|LineDirectiveTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.LineDirectiveTrivia ->
            let t = n :?> LineDirectiveTriviaSyntax
            Some(t.Line, t.File, t.IsActive)
        | _ -> None

    let (|PragmaWarningDirectiveTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? PragmaWarningDirectiveTriviaSyntax as t -> Some(t.Kind(), t.HashToken, t.PragmaKeyword, t.WarningKeyword, t.DisableOrRestoreKeyword, t.ErrorCodes, t.EndOfDirectiveToken, t.IsActive)
        | _ -> None

    let (|PragmaWarningDirectiveTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.PragmaWarningDirectiveTrivia ->
            let t = n :?> PragmaWarningDirectiveTriviaSyntax
            Some(t.ErrorCodes, t.IsActive)
        | _ -> None

    let (|PragmaChecksumDirectiveTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? PragmaChecksumDirectiveTriviaSyntax as t -> Some(t.Kind(), t.HashToken, t.PragmaKeyword, t.ChecksumKeyword, t.File, t.Guid, t.Bytes, t.EndOfDirectiveToken, t.IsActive)
        | _ -> None

    let (|PragmaChecksumDirectiveTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.PragmaChecksumDirectiveTrivia ->
            let t = n :?> PragmaChecksumDirectiveTriviaSyntax
            Some(t.File, t.Guid, t.Bytes, t.IsActive)
        | _ -> None

    let (|ReferenceDirectiveTriviaSyntax|_|) (n: CSharpSyntaxNode) =
        match n with
        | :? ReferenceDirectiveTriviaSyntax as t -> Some(t.Kind(), t.HashToken, t.ReferenceKeyword, t.File, t.EndOfDirectiveToken, t.IsActive)
        | _ -> None

    let (|ReferenceDirectiveTrivia|_|) (n: CSharpSyntaxNode) =
        match n.Kind() with
        | SyntaxKind.ReferenceDirectiveTrivia ->
            let t = n :?> ReferenceDirectiveTriviaSyntax
            Some(t.File, t.IsActive)
        | _ -> None